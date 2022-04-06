using ImageProcessing.Core;
using ImageProcessing.Core.Processers;
using ImageProcessing.Utils;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Data;
using System.Dynamic;
using System.Collections.Generic;

namespace ImageProcessing {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public static MainWindow instance;

        private ImageData imgOriginal;
        public ImageData ImageOriginal => imgOriginal;

        private ImageData imgProcessed;
        public ImageData ImageProcessed => imgProcessed;

        private ImageProcesser imgProcesser;
        private GraphDisplayer graphDisplayer;

        readonly string[] matrixDisplayOperations = new string[] {
            "Pixels",
            "RGB",
            "R",
            "G",
            "B",
            "A4 matrix",
            "A8 matrix",
            "Manhattan Distance",
        };

        readonly string[] matrixPerformOperations = new string[] {
            "Convert to grey",
            "Convert to binary threshold",
            "Convert to binary S9",
            "Invert colors"
        };

        readonly string[] graphDisplayOptions = new string[] {
            "Manhattan Distance",
            "Brightness",
        };

        public MainWindow() {
            InitializeComponent();
            UI_Utility.InitComboBoxMenu(matrixDropMenu, matrixDisplayOperations);
            UI_Utility.InitComboBoxMenu(performDropMenu, matrixPerformOperations);
            UI_Utility.InitComboBoxMenu(graphDropMenu, graphDisplayOptions);
            instance = this;

            imageMatrix.IsReadOnly = true;
            graphDisplayer = new GraphDisplayer(Graph);
        }

        private void OnBtnPerformClick(object sender, RoutedEventArgs e) {
            if (imgOriginal != null) {
                switch (performDropMenu.SelectedItem) {
                    case "Convert to grey":
                        imgProcesser = new ConvertToGrey();
                        break;
                    case "Convert to binary threshold":
                        imgProcesser = new ConvertToBinaryThreshold();
                        break;
                    case "Convert to binary S9":
                        imgProcesser = new ConvertToBinaryS9();
                        break;
                    case "Invert colors":
                        imgProcesser = new InvertColor();
                        break;
                }

                try {
                    imgProcessed = imgProcesser.Process(new ImageData(imgOriginal));
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                UpdateProcessedImageUI(imgProcessed);
            }
        }

        private void OnBtnCopyToOriginalClick(object sender, RoutedEventArgs e) {
            if (imgProcessed == null) {
                MessageBox.Show("Can't copy processed image to replace original - it's null!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            UpdateOriginalImageUI(imgProcessed);
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
                    imgOriginal = new ConvertToBinaryThreshold().Process(imgOriginal);
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
                MessageBox.Show($"Failed to save result image (it's null)", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public void UpdateProcessedImageUI(ImageData image) {
            if (image == null) {
                return;
            }
            imgProcessed = image;
            workSpaceImage.Source = image.GetBitmapImage();
            imageCanvas.Width = image.Width;
            imageCanvas.Height = image.Height;
        }

        public void UpdateOriginalImageUI(ImageData image) {
            imgOriginal = image;
            workSpaceImage_original.Source = image.GetBitmapImage();
            imageCanvas_original.Width = image.Width;
            imageCanvas_original.Height = image.Height;
        }

        private void btnMatrixDisplay_Click(object sender, RoutedEventArgs e) {
            ImageData image = (bool)btnMatrixToggle.IsChecked ? imgOriginal : imgProcessed;

            if (image == null) {
                return;
            }

            switch (matrixDropMenu.SelectedItem.ToString()) {
                case "Pixels":
                    DisplayMatrix(image.GetBinaryPixelsInverted(), image.Width, image.Height);
                    break;
                case "RGB":
                    DisplayMatrix(image.GetPixelsRGB(), image.Width, image.Height);
                    break;
                case "R":
                    DisplayMatrix(imgOriginal.GetChannelValuesString(0x00ff0000, 16),
                        image.Width, image.Height);
                    break;
                case "G":
                    DisplayMatrix(imgOriginal.GetChannelValuesString(0x0000ff00, 8),
                        image.Width, image.Height);
                    break;
                case "B":
                    DisplayMatrix(imgOriginal.GetChannelValuesString(0x000000ff, 0),
                        image.Width, image.Height);
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
            if (imgOriginal == null) {
                MessageBox.Show("Image original in null! Load it first!", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<OxyPlot.DataPoint> points = new List<OxyPlot.DataPoint>();
            int[] dataArray = null;
            switch (graphDropMenu.SelectedItem.ToString()) {
                case "Manhattan Distance":
                    dataArray = ImageMatrixCalculator.GetManhattanDistanceMatrix(imgOriginal);
                    break;
                case "Brightness":
                    dataArray = imgOriginal.GetBrightnessArray();
                    break;
            }

            points = ImageMatrixCalculator.CalculateValuesFrequences(dataArray);
            graphDisplayer.DisplayColumnGraph(points);
        }
    }
}
