using ImageProcessing.Code.Core;
using ImageProcessing.Utils;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Data;
using System.Dynamic;
using System.Collections.Generic;
using ImageProcessing.Core;

namespace ImageProcessing {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private ImageData imgOriginal;
        private ImageData imgProcessed;
        private ImageProcesser imgProcesser;

        readonly string[] matrixDisplayOperations = new string[] {
            "Pixels",
            "A4 matrix",
            "A8 matrix",
        };

        public MainWindow() {
            InitializeComponent();
            InitMatrixDropDownMenu();
        }

        private void InitMatrixDropDownMenu() {
            foreach (string item in matrixDisplayOperations) {
                matrixDropMenu.Items.Add(item);
            }
        }

        private void OnBtnPerformClick(object sender, RoutedEventArgs e) {
            if (imgOriginal != null && imgProcesser != null) {
                imgProcessed = imgProcesser.Process(imgOriginal);
                workSpaceImage.Source = imgProcessed.GetBitmapImage();
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

        private void btnMatrixDisplay_Click(object sender, RoutedEventArgs e) {
            switch (matrixDropMenu.SelectedItem.ToString()) {
                case "Pixels":
                    DisplayMatrix(imgOriginal.GetPixels(), imgOriginal.Width, imgOriginal.Height);
                    break;
                case "A4 matrix":
                    DisplayMatrix(ImageMatrixCalculator.GetA4Matrix(imgOriginal), 
                        imgOriginal.Width, imgOriginal.Height);
                    break;
                case "A8 matrix":
                    DisplayMatrix(ImageMatrixCalculator.GetA8Matrix(imgOriginal),
                        imgOriginal.Width, imgOriginal.Height);
                    break;
            }
            
        }

        private void DisplayMatrix(int[] matrix, int width, int height) {
            imageMatrix.Columns.Clear();
            imageMatrix.Items.Clear();

            string[] labels = new string[width];
            for(int i = 0; i < width; i++) {
                labels[i] = i.ToString();

                DataGridTextColumn col = new DataGridTextColumn();
                col.Header = labels[i];
                col.Binding = new Binding(labels[i]);

                imageMatrix.Columns.Add(col);

            }

            for(int y = 0; y < height; y++) {
                dynamic row = new ExpandoObject();
                for (int x = 0; x < width; x++) {
                    ((IDictionary<string, object>)row)[labels[x]] = matrix[y * width + x];
                }
                imageMatrix.Items.Add(row);
            }
        }
    }
}
