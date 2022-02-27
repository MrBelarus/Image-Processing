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
                BitmapImage image = ImageManager.LoadImage(openFileDialog.FileName);
                if (image == null) {
                    MessageBox.Show("Failed to load image", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
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
