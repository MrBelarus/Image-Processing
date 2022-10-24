using ImageProcessing.Core;
using ImageProcessing.Core.Processers;
using ImageProcessing.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
                imgName = openFileDialog.FileName;
            }

            CalculateImageSpecialPoints();
        }

        private void LoadImage(string fileName) {
            imgOriginal = new ImageData(fileName);
            if (imgOriginal.ColorDepth != 1) {
                
                imgOriginal = ImageUtility.ConvertToBinary(imgOriginal, true);//new ConvertToBinaryThreshold().Process(imgOriginal);
                imgOriginal = new ZhangSuen().Process(new ImageData(imgOriginal));
            }

            if (imgOriginal == null) {
                MessageBox.Show("Failed to load image", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
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
                imgPath = "letters\\" + imgName,
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
