using ImageProcessing.Core;
using ImageProcessing.Core.Processers;
using ImageProcessing.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ImageProcessing {
    /// <summary>
    /// Логика взаимодействия для ImageDetectWindow.xaml
    /// </summary>
    public partial class ImageDetectWindow : Window {
        private ImageData imgOriginal;
        private string imgName;
        private GridDisplayer imgClassTable;

        public ImageDetectWindow() {
            InitializeComponent();

            imgClassTable = new GridDisplayer(classTable);
        }

        private void btnLoadImg_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                //load & remember bitmap
                LoadImage(openFileDialog.FileName);
                imgName = openFileDialog.SafeFileName;

                ImageDetectDataProvider.Instance.CopyImgToBin(openFileDialog.FileName, openFileDialog.SafeFileName);
            }
            else {
                return;
            }

            CalculateImageSpecialPoints();
            CalculateZondValues();
        }

        int zondNothingColor = 0x00FFFFFF;
        private void LoadImage(string fileName) {
            imgOriginal = new ImageData(fileName);
            if (imgOriginal == null) {
                MessageBox.Show("Failed to load image", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            if (imgOriginal.ColorDepth != 1) {

                imgOriginal = ImageUtility.ConvertToBinary(imgOriginal, true, 128); //new ConvertToBinaryThreshold().Process(imgOriginal);
                imgOriginal = new ZhangSuen().Process(new ImageData(imgOriginal));
            }

            if (ImageDetectDataProvider.Instance.zondBlue.IsImageLoaded) {
                ImageData zondImgOverlay = new ImageData(imgOriginal);
                zondImgOverlay = ImageUtility.Convert1BitToGray24Bit(zondImgOverlay);
                int[] pixels = imgOriginal.GetPixels(),
                    zondBluePixels = ImageDetectDataProvider.Instance.zondBlue.GetPixels(),
                    zondRedPixels = ImageDetectDataProvider.Instance.zondRed.GetPixels();
                for (int i = 0; i < pixels.Length; i++) {
                    bool hasPixel = pixels[i] == 0;
                    if (zondBluePixels[i] != zondNothingColor) {
                        pixels[i] = hasPixel ? zondBluePixels[i] / 2 : zondBluePixels[i];
                    }
                    if (zondRedPixels[i] != zondNothingColor) {
                        pixels[i] = hasPixel ? zondRedPixels[i] / 2 : zondRedPixels[i];
                    }
                }
                zondImgOverlay.SetPixels(pixels);
                zondImgOverlay.ApplyChanges();

                UpdateOriginalImageUI(zondImgOverlay);
            }
            else {
                UpdateOriginalImageUI(imgOriginal);
            }
        }

        private void CalculateImageSpecialPoints() {
            int[] matrixCn = ImageMatrixCalculator.GetCnMatrix(imgOriginal, true);
            int Ny = 0, Nk = 0;
            foreach (int Cn in matrixCn) {
                if (Cn == 1) {
                    Nk++;
                }
                else if (Cn > 2) {
                    Ny++;
                }
            }

            txtNk.Text = "Nk :\t" + Nk.ToString();
            txtNy.Text = "Ny :\t" + Ny.ToString();
        }

        private void CalculateZondValues() {
            var detectProvider = ImageDetectDataProvider.Instance;


            int[] pixels = imgOriginal.GetPixels();
            int[] redZondPixels = detectProvider.zondRed.GetPixels();
            int[] blueZondPixels = detectProvider.zondBlue.GetPixels();

            if (pixels.Length != redZondPixels.Length || blueZondPixels.Length != pixels.Length) {
                MessageBox.Show("Image size should be same as zond size", "error");
                return;
            }

            int blueZondCounter = 0, redZondCounter = 0;
            for (int i = 0; i < pixels.Length; i++) {
                if (pixels[i] != 0) {
                    continue;
                }

                if (blueZondPixels[i] != zondNothingColor) {
                    blueZondCounter++;
                }
                if (redZondPixels[i] != zondNothingColor) {
                    redZondCounter++;
                }
            }

            txtZondBlue.Text = "Blue z\t" + blueZondCounter.ToString();
            txtZondRed.Text = "Red z\t" + redZondCounter.ToString();
        }

        private void btnSaveToDB_Click(object sender, RoutedEventArgs e) {
            if (txtClass.Text == "Class" || txtClass.Text == string.Empty) {
                MessageBox.Show("Class name not set!", "error");
                return;
            }


            ImageDetectDataProvider.Instance.AddData(new ImageDetectData() {
                className = txtClass.Text,
                nodesBranchesCount = int.Parse(txtNy.Text.Split('\t')[1]),
                nodesEndCount = int.Parse(txtNk.Text.Split('\t')[1]),
                zondBlueIntersectCount = int.Parse(txtZondBlue.Text.Split('\t')[1]),
                zondRedIntersectCount = int.Parse(txtZondRed.Text.Split('\t')[1]),
                imgName = imgName,
            });

            MessageBox.Show("Save was successful!");
            ImageDetectDataProvider.Instance.SaveData();
        }

        private void btnDetectClass_Click(object sender, RoutedEventArgs e) {
            if (imgOriginal == null) {
                btnLoadImg_Click(sender, e);
            }
            else {
                CalculateImageSpecialPoints();
            }

            int nodesBranchesCount = int.Parse(txtNy.Text.Split('\t')[1]);
            int nodesEndCount = int.Parse(txtNk.Text.Split('\t')[1]);
            int k = 3;

            var detectInfoImgs = ImageDetectDataProvider.Instance.ImageDetectDatas;
            for (int i = 0; i < detectInfoImgs.Length; i++) {
                detectInfoImgs[i].distance = (float)Math.Sqrt(
                    Math.Pow(nodesEndCount - detectInfoImgs[i].nodesEndCount, 2) +
                    Math.Pow(nodesBranchesCount - detectInfoImgs[i].nodesBranchesCount, 2));
            }

            List<ImageDetectData> sortedByDist = new List<ImageDetectData>(detectInfoImgs);
            sortedByDist = new List<ImageDetectData>(sortedByDist.OrderBy(img => img.distance));
            Dictionary<string, int> detectDataCount = new Dictionary<string, int>();
            bool allDifferent = true;
            for (int i = 0; i < k; i++) {
                if (detectDataCount.ContainsKey(sortedByDist[i].className)) {
                    detectDataCount[sortedByDist[i].className]++;
                }
                else {
                    detectDataCount.Add(sortedByDist[i].className, 1);
                }
            }

            int maxValue = int.MinValue;
            string maxClass = string.Empty;
            foreach (var keypair in detectDataCount) {
                if (keypair.Value > maxValue) {
                    maxValue = keypair.Value;
                    maxClass = keypair.Key;
                }
            }

            int count = 0;
            foreach (var keypair in detectDataCount) {
                if (keypair.Value == maxValue) {
                    count++;
                    if (count > 1) {
                        allDifferent = false;
                        break;
                    }
                }
            }

            if (allDifferent) {
                txtClass.Text = maxClass;
                MessageBox.Show("Class was detected! It's " + maxClass);
            }
            else {
                MessageBox.Show("Can't detect class, multiple max distances!");
            }
        }

        private void btnTableDisplay_Click(object sender, RoutedEventArgs e) {
            var repoImgs = ImageDetectDataProvider.Instance.ImageDetectDatas;
            string[] colomnNames = new string[] { "Ny", "Nk", "D" };
            int columnsCount = ImageDetectData.TotalVariables;
            string[] rowNames = new string[repoImgs.Length];
            float[][] dataArray = new float[repoImgs.Length][];

            for (int i = 0; i < dataArray.Length; i++) {
                dataArray[i] = new float[columnsCount];

                var img = repoImgs[i];
                dataArray[i][0] = img.nodesBranchesCount;
                dataArray[i][1] = img.nodesEndCount;
                dataArray[i][2] = img.distance;
                rowNames[i] = img.className;
            }

            float[] tableData = new float[columnsCount * dataArray.Length];
            int index = 0;
            foreach (float[] line in dataArray) {
                foreach (float value in line) {
                    tableData[index] = value;
                    index++;
                }
            }

            imgClassTable.DisplayMatrix<float>(tableData, columnsCount, rowNames.Length,
                                                    rowNames, colomnNames);
        }

        public void UpdateOriginalImageUI(ImageData image) {
            imgOriginal = image;
            UpdateImageSize(image, this.workSpaceImage, this.imageCanvas);
        }

        private void UpdateImageSize(ImageData imgData, Image workSpaceImage, Canvas imgCanvas) {
            workSpaceImage.Source = imgData.GetBitmapImage();

            if (imgData.Width < 100) {
                workSpaceImage.Width = imgData.Width * 4;
                workSpaceImage.Height = imgData.Height * 4;
            }
            else if (imgData.Width < 250) {
                workSpaceImage.Width = imgData.Width * 2;
                workSpaceImage.Height = imgData.Height * 2;
            }
            else {
                workSpaceImage.Width = imgData.Width;
                workSpaceImage.Height = imgData.Height;
            }

            imgCanvas.Width = workSpaceImage.Width;
            imgCanvas.Height = workSpaceImage.Height;
        }
    }
}
