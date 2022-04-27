﻿using ImageProcessing.Core;
using ImageProcessing.Core.Processers;
using ImageProcessing.Utils;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
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
        private GridDisplayer gridDisplayer;

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
            "Invert colors",
            "RGB to grey max",
            "Filter 3x3"
        };

        readonly string[] graphDisplayOptions = new string[] {
            "Manhattan Distance",
            "Brightness",
            "R",
            "G",
            "B",
        };

        public MainWindow() {
            InitializeComponent();
            UI_Utility.InitComboBoxMenu(matrixDropMenu, matrixDisplayOperations);
            UI_Utility.InitComboBoxMenu(performDropMenu, matrixPerformOperations);
            UI_Utility.InitComboBoxMenu(graphDropMenu, graphDisplayOptions);
            instance = this;

            graphDisplayer = new GraphDisplayer(Graph);
            gridDisplayer = new GridDisplayer(imageMatrix);
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
                    case "RGB to grey max":
                        imgProcesser = new ConvertRGB_ToGreyMax();
                        break;
                    case "Filter 3x3":
                        imgProcesser = new AverageFiltration3x3(new int[3][] {
                            new int[] {1, 1, 1},
                            new int[] {1, 1, 1},
                            new int[] {1, 1, 1},
                        });
                        break;
                }

                try {
                    ImageData img = imgProcesser.Process(new ImageData(imgOriginal));
                    if (img == null)
                        return;
                    imgProcessed = img;
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

                UpdateOriginalImageUI(ImageOriginal);
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
            if (image.Width < 50) {
                workSpaceImage.Width = image.Width * 4;
                workSpaceImage.Height = image.Height * 4;
            }
            else if (image.Width < 100) {
                workSpaceImage.Width = image.Width * 2;
                workSpaceImage.Height = image.Height * 2;
            }
            imageCanvas.Width = workSpaceImage.Width;
            imageCanvas.Height = workSpaceImage.Height;
        }

        public void UpdateOriginalImageUI(ImageData image) {
            imgOriginal = image;
            workSpaceImage_original.Source = image.GetBitmapImage();
            if (image.Width < 50) {
                workSpaceImage_original.Width = image.Width * 4;
                workSpaceImage_original.Height = image.Height * 4;
            }
            else if (image.Width < 100) {
                workSpaceImage_original.Width = image.Width * 2;
                workSpaceImage_original.Height = image.Height * 2;
            }
            imageCanvas_original.Width = workSpaceImage_original.Width;
            imageCanvas_original.Height = workSpaceImage_original.Height;
        }

        private void btnMatrixDisplay_Click(object sender, RoutedEventArgs e) {
            ImageData image = (bool)btnMatrixToggle.IsChecked ? imgOriginal : imgProcessed;

            if (image == null) {
                return;
            }

            switch (matrixDropMenu.SelectedItem.ToString()) {
                case "Pixels":
                    gridDisplayer.DisplayMatrix(image.GetBinaryPixelsInverted(), image.Width, image.Height);
                    break;
                case "RGB":
                    gridDisplayer.DisplayMatrix(image.GetPixelsRGB(), image.Width, image.Height);
                    break;
                case "R":
                    gridDisplayer.DisplayMatrix(imgOriginal.GetChannelValuesString(0x00ff0000, 16),
                        image.Width, image.Height);
                    break;
                case "G":
                    gridDisplayer.DisplayMatrix(imgOriginal.GetChannelValuesString(0x0000ff00, 8),
                        image.Width, image.Height);
                    break;
                case "B":
                    gridDisplayer.DisplayMatrix(imgOriginal.GetChannelValuesString(0x000000ff, 0),
                        image.Width, image.Height);
                    break;
                case "A4 matrix":
                    gridDisplayer.DisplayMatrix(ImageMatrixCalculator.GetA4Matrix(image),
                        image.Width, image.Height);
                    break;
                case "A8 matrix":
                    gridDisplayer.DisplayMatrix(ImageMatrixCalculator.GetA8Matrix(image),
                        image.Width, image.Height);
                    break;
                case "Manhattan Distance":
                    gridDisplayer.DisplayMatrix(ImageMatrixCalculator.GetManhattanDistanceMatrix(image),
                        image.Width, image.Height);
                    break;
            }
        }

        private void btnMatrixApply_Click(object sender, RoutedEventArgs e) {
            ImageData image = (bool)btnMatrixToggle.IsChecked ? imgOriginal : imgProcessed;

            if (image == null) {
                return;
            }

            switch (matrixDropMenu.SelectedItem.ToString()) {
                case "R":
                    gridDisplayer.ApplyChangesInChannel(image, 0x00FFFF, 16);
                    break;
                case "G":
                    gridDisplayer.ApplyChangesInChannel(image, 0xFF00FF, 8);
                    break;
                case "B":
                    gridDisplayer.ApplyChangesInChannel(image, 0xFFFF00, 0);
                    break;
                default:
                    gridDisplayer.ApplyChanges(image);
                    break;
            }

            if ((bool)btnMatrixToggle.IsChecked) {
                UpdateOriginalImageUI(image);
            }
            else {
                UpdateProcessedImageUI(image);
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
                case "R":
                    dataArray = imgOriginal.GetChannelValues(0xff0000, 16);
                    break;
                case "G":
                    dataArray = imgOriginal.GetChannelValues(0x00ff00, 8);
                    break;
                case "B":
                    dataArray = imgOriginal.GetChannelValues(0x0000ff, 0);
                    break;
            }

            points = ImageMatrixCalculator.CalculateValuesFrequences(dataArray);
            graphDisplayer.DisplayColumnGraph(points);
        }
    }
}
