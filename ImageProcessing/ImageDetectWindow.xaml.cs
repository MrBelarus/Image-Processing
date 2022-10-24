using ImageProcessing.Core;
using ImageProcessing.Core.Processers;
using ImageProcessing.Utils;
using Microsoft.Win32;
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
            
        }

        private void btnTableDisplay_Click(object sender, RoutedEventArgs e) {
            var repoImgs = ImageDetectDataProvider.Instance.ImageDetectDatas;
            int columnsCount = ImageDetectData.TotalVariables;
            string[] colomnNames = new string[] { "Ny", "Nk" };
            string[] rowNames = new string[repoImgs.Length];
            float[][] dataArray = new float[repoImgs.Length][];

            for (int i = 0; i < dataArray.Length; i++) {
                dataArray[i] = new float[columnsCount];

                var img = repoImgs[i];
                dataArray[i][0] = img.nodesBranchesCount;
                dataArray[i][1] = img.nodesEndCount;
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
