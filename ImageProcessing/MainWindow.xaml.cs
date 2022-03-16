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
        private GraphDisplayer graphDisplayer;

        readonly string[] matrixDisplayOperations = new string[] {
            "Pixels",
            "RGB",
            "A4 matrix",
            "A8 matrix",
            "Manhattan Distance",
        };

        readonly string[] matrixPerformOperations = new string[] {
            "Convert to grey",
        };

        public MainWindow() {
            InitializeComponent();
            InitDropDownMenu(matrixDropMenu, matrixDisplayOperations);
            InitDropDownMenu(performDropMenu, matrixPerformOperations);

            imageMatrix.IsReadOnly = true;
            graphDisplayer = new GraphDisplayer(Graph);

            imgProcesser = new InvertColor();
            //imgProcesser = new TestProcesser();
        }

        private void InitDropDownMenu(ComboBox menu, string[] items) {
            foreach (string item in items) {
                menu.Items.Add(item);
            }
            menu.SelectedIndex = 0;
        }

        private void OnBtnPerformClick(object sender, RoutedEventArgs e) {
            if (imgOriginal != null && imgProcesser != null) {
                switch (performDropMenu.SelectedItem) {
                    default:
                        imgProcessed = ImageUtility.Convert1BitToGray24Bit(imgOriginal);
                        break;
                }
                //imgProcessed = imgProcesser.Process(new ImageData(imgOriginal));
                workSpaceImage.Source = imgProcessed.GetBitmapImage();
                imageCanvas.Width = imgOriginal.Width;
                imageCanvas.Height = imgOriginal.Height;
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
                if (imgOriginal.ColorDepth != 1 && (bool)btnConvertBinaryOriginal.IsChecked) {
                    imgOriginal = ImageUtility.ConvertToBinary(imgOriginal);
                }

                if (imgOriginal == null) {
                    MessageBox.Show("Failed to load image", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                workSpaceImage_original.Width = imgOriginal.Width;
                workSpaceImage_original.Height = imgOriginal.Height;
                workSpaceImage_original.Source = imgOriginal.GetBitmapImage();

                //refresh width/height of canvas
                imageCanvas_original.Width = imgOriginal.Width;
                imageCanvas_original.Height = imgOriginal.Height;
            }
        }

        private void OnMenuSaveFileClick(object sender, RoutedEventArgs e) {
            if (workSpaceImage.Source == null) {
                MessageBox.Show($"Failed to save image (it's null)", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true) {
                try {
                    ImageUtility.SaveImage(workSpaceImage, saveFileDialog.FileName, new BmpBitmapEncoder());
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
            ImageData image = (bool)btnMatrixToggle.IsChecked ? imgOriginal : imgProcessed;

            if(image == null) {
                return;
            }

            switch (matrixDropMenu.SelectedItem.ToString()) {
                case "Pixels":
                    DisplayMatrix(image.GetBinaryPixelsInverted(), image.Width, image.Height);
                    break;
                case "RGB":
                    DisplayMatrix(image.GetPixelsRGB(), image.Width, image.Height);
                    break;
                case "A4 matrix":
                    DisplayMatrix(ImageMatrixCalculator.GetA4Matrix(image),
                        image.Width, image.Height);
                    break;
                case "A8 matrix":
                    DisplayMatrix(ImageMatrixCalculator.GetA8Matrix(image),
                        image.Width, image.Height);
                    break;
                case "Manhattan Distance":
                    DisplayMatrix(ImageMatrixCalculator.GetManhattanDistanceMatrix(image),
                        image.Width, image.Height);
                    break;
            }

        }

        private void DisplayMatrix<T>(T[] matrix, int width, int height) {
            imageMatrix.Columns.Clear();
            imageMatrix.Items.Clear();

            string[] labels = new string[width];
            for (int i = 0; i < width; i++) {
                labels[i] = i.ToString();

                DataGridTextColumn col = new DataGridTextColumn();
                col.Header = labels[i];
                col.Binding = new Binding(labels[i]);

                imageMatrix.Columns.Add(col);

            }

            for (int y = 0; y < height; y++) {
                dynamic row = new ExpandoObject();
                for (int x = 0; x < width; x++) {
                    ((IDictionary<string, object>)row)[labels[x]] = matrix[y * width + x];
                }
                
                imageMatrix.Items.Add(row);
            }
        }

        private void btnGraphDisplay_Click(object sender, RoutedEventArgs e) {
            var result = ImageMatrixCalculator.GetManhattanDistanceMatrix(imgOriginal);
            graphDisplayer.DisplayColumnGraph(ImageMatrixCalculator.CalculateValuesFrequences(result));
        }
    }
}
