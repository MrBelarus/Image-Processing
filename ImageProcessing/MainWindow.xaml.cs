using ImageProcessing.Code.Core;
using ImageProcessing.Utils;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageProcessing {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private System.Drawing.Bitmap imgOriginalBitmap;

        public MainWindow() {
            InitializeComponent();
        }

        private void OnBtnPerformClick(object sender, RoutedEventArgs e) {
            
        }

        #region MenuBarActions
        private void OnMenuCreateNewClick(object sender, RoutedEventArgs e) {
            workSpaceImage_original.Source = null;
            workSpaceImage.Source = null;
        }

        private void OnMenuOpenFileClick(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                //load & remember bitmap
                imgOriginalBitmap = ImageManager.LoadImage(openFileDialog.FileName);
                if (imgOriginalBitmap == null) {
                    MessageBox.Show("Failed to load image", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                ImgToGrey imgToGrey = new ImgToGrey();
                imgOriginalBitmap = imgToGrey.Process(imgOriginalBitmap);

                //convert bitmap to bitmapImage for wpf image component
                BitmapImage image = ImageManager.BitmapToImageSource(imgOriginalBitmap);
                workSpaceImage_original.Source = image;

                //refresh width/height of canvas
                imageCanvas_original.Width = image.Width;
                imageCanvas_original.Height = image.Height;
            }
        }

        private void OnMenuSaveFileClick(object sender, RoutedEventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true) {
                try {
                    ImageManager.SaveImage(workSpaceImage_original, saveFileDialog.FileName, new BmpBitmapEncoder());
                }
                catch (Exception ex) {
                    MessageBox.Show($"{ex.Message}\n Failed to save image", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void OnMenuExitClick(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
        #endregion
    }
}
