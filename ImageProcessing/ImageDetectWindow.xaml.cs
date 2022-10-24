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

        public ImageDetectWindow() {
            InitializeComponent();
        }

        private void btnLoadImg_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                //load & remember bitmap
                LoadImage(openFileDialog.FileName);
                imgName = openFileDialog.SafeFileName;

                ImageDetectDataProvider.Instance.CopyImgToBin(openFileDialog.FileName, openFileDialog.SafeFileName);
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
                if(Cn == 1) {
                    Nk++;
                }
                else if(Cn > 2) {
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
