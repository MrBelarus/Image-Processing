using ImageProcessing.Core;
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
    /// Interaction logic for TextureAnalyse.xaml
    /// </summary>
    public partial class TextureAnalyse : Window {

        public TextureAnalyse() {
            InitializeComponent();
        }

        public void SetImage(ImageData imageData) {
            UpdateImageSize(imageData, this.workSpaceImage, this.imageCanvas);
            
            var matrix = CoOccurenceMatrixCalculator.GetNormalizedMatrix(
                                                CoOccurenceMatrixCalculator.GetImageMatrix_C90_D1(imageData));

            this.contrastText.Text = CoOccurenceMatrixCalculator.GetContrast(matrix).ToString();
            this.energyText.Text = CoOccurenceMatrixCalculator.GetEnergy(matrix).ToString();
            this.entrophyText.Text = CoOccurenceMatrixCalculator.GetEntrophy(matrix).ToString();
            this.homogenText.Text = CoOccurenceMatrixCalculator.GetHomogen(matrix).ToString();
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
