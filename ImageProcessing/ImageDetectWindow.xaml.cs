using Aspose.Cells;
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
        }

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

            UpdateOriginalImageUI(imgOriginal);
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

        private void btnSaveToDB_Click(object sender, RoutedEventArgs e) {
            if (txtClass.Text == "Class" || txtClass.Text == string.Empty) {
                MessageBox.Show("Class name not set!", "error");
                return;
            }

            ImageDetectDataProvider.Instance.AddData(new ImageDetectData() {
                className = txtClass.Text,
                nodesBranchesCount = int.Parse(txtNy.Text.Split('\t')[1]),
                nodesEndCount = int.Parse(txtNk.Text.Split('\t')[1]),
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
            for(int i = 0; i < detectInfoImgs.Length; i++) {
                detectInfoImgs[i].distance = (float)Math.Sqrt(
                    Math.Pow(nodesEndCount - detectInfoImgs[i].nodesEndCount, 2) + 
                    Math.Pow(nodesBranchesCount - detectInfoImgs[i].nodesBranchesCount, 2));
            }

            List<ImageDetectData> sortedByDist = new List<ImageDetectData>(detectInfoImgs);
            sortedByDist.OrderBy(img => img.distance);
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
            foreach(var keypair in detectDataCount) {
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

        private void btnChart_Click(object sender, RoutedEventArgs e) {
            CreateChart(ImageDetectDataProvider.Instance.ImageDetectDatas);
        }

        private void CreateChart(ImageDetectData[] dots) {
            // Instantiate a Workbook object
            Workbook workbook = new Workbook();

            // Obtain the reference of the first worksheet
            Worksheet worksheet = workbook.Worksheets[0];


            worksheet.Cells["A1"].PutValue("Ny");
            worksheet.Cells["B1"].PutValue("Nk");
            worksheet.Cells["C1"].PutValue("Class");

            for (int i = 0; i < dots.Length; i++) {
                worksheet.Cells["A" + (2 + i).ToString()].PutValue(dots[i].nodesBranchesCount);
                worksheet.Cells["B" + (2 + i).ToString()].PutValue(dots[i].nodesEndCount);
                worksheet.Cells["C" + (2 + i).ToString()].PutValue(dots[i].className);
            }

            // Add a chart to the worksheet
            int chartIndex = worksheet.Charts.Add(Aspose.Cells.Charts.ChartType.Scatter, 5, 0, 15, 5);

            // Access the instance of the newly added chart
            Aspose.Cells.Charts.Chart chart = worksheet.Charts[chartIndex];

            // Set chart data source as the range  "A1:C4"
            chart.SetChartDataRange("A1:B" + (1 + dots.Length).ToString(), true);

            // Save the Excel file
            workbook.Save("Column-Chart.xls");
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
                foreach(float value in line) {
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
