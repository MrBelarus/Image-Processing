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
        private ImageData imgOriginal;
        private ImageData imgProcessed;

        public MainWindow() {
            InitializeComponent();
        }

        private void OnBtnPerformClick(object sender, RoutedEventArgs e) {
            if (imgOriginal != null) {
                imgOriginal = new InvertColor().Process(imgOriginal);
                workSpaceImage_original.Source = imgOriginal.GetBitmapImage();
            }
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
                imgOriginal = new ImageData(openFileDialog.FileName);
                if (imgOriginal == null) {
                    MessageBox.Show("Failed to load image", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                workSpaceImage_original.Source = imgOriginal.GetBitmapImage();

                //refresh width/height of canvas
                imageCanvas_original.Width = imgOriginal.Width;
                imageCanvas_original.Height = imgOriginal.Height;
            }
        }

        private void OnMenuSaveFileClick(object sender, RoutedEventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true) {
                try {
                    ImageUtility.SaveImage(workSpaceImage_original, saveFileDialog.FileName, new BmpBitmapEncoder());
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
