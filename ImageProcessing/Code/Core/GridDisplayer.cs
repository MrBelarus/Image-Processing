using ImageProcessing.Core;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageProcessing.Core {
    public class GridDisplayer {
        private DataGrid imageMatrix;

        public GridDisplayer(DataGrid grid) {
            imageMatrix = grid;
            imageMatrix.IsReadOnly = true;

            imageMatrix.MouseDoubleClick += HandleDoubleClick;
        }

        int[] matrix = null;
        int width;

        private void HandleDoubleClick(object sender, MouseButtonEventArgs e) {
            if (imageMatrix.CurrentColumn == null || imageMatrix.SelectedIndex < 0 ||
                matrix == null) {
                return;
            }

            ExpandoObject selectedItem = imageMatrix.SelectedValue as ExpandoObject;
            int y = imageMatrix.SelectedIndex;
            int x = imageMatrix.CurrentColumn.DisplayIndex;

            if (selectedItem == null)
                return;

            InputValueWindow window = new InputValueWindow();

            window.ShowDialog();

            if (window.doApply) {
                matrix[y * width + x] = (int)window.Value;
                ((IDictionary<string, object>)selectedItem)[x.ToString()] = window.Value.ToString();
            }

        }

        public void DisplayMatrix<T>(T[] matrix, int width, int height) {
            imageMatrix.Columns.Clear();
            imageMatrix.Items.Clear();

            try {
                this.width = width;
                this.matrix = new int[matrix.Length];
                for (int i = 0; i < matrix.Length; i++) {
                    this.matrix[i] = int.Parse(matrix[i].ToString());
                }
            }
            catch (Exception e) {
                this.matrix = null;
            }

            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "";
            col1.Binding = new Binding("-1");

            imageMatrix.Columns.Add(col1);
            for (int x = 0; x < width; x++) { 
                DataGridTextColumn col = new DataGridTextColumn();
                col.Header = x.ToString();
                col.Binding = new Binding(x.ToString());

                imageMatrix.Columns.Add(col);

            }

            for (int y = 0; y < height; y++) {
                dynamic row = new ExpandoObject();
                ((IDictionary<string, object>)row)["-1"] = y.ToString();

                for (int x = 0; x < width; x++) {
                    ((IDictionary<string, object>)row)[x.ToString()] = matrix[y * width + x].ToString();
                }
                imageMatrix.Items.Add(row);
            }
        }

        private DataGridCell GetCell(int rowIndex, int columnIndex) {
            var row = GetRow(rowIndex);
            var p = GetVisualChild<DataGridCellsPresenter>(row);
            if(p == null) {
                return null;
            }
            return p.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
        }

        private DataGridRow GetRow(int index) {
            var row = (DataGridRow)imageMatrix.ItemContainerGenerator.ContainerFromIndex(index);
            if(row == null) {
                imageMatrix.UpdateLayout();
                imageMatrix.ScrollIntoView(imageMatrix.Items[index]);
                row = GetRow(index);
            }
            return row;
        }

        private T GetVisualChild<T>(Visual parent) where T: Visual {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for(int i = 0; i < numVisuals; i++) {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null) {
                    child = GetVisualChild<T>(v);
                }
                if(child != null) {
                    break;
                }
            }

            return child;
        }

        public void ChangeColorForCnMatrix(int[] matrix, int width, int height) {

            imageMatrix.EnableColumnVirtualization = false;
            imageMatrix.EnableRowVirtualization = false;
            imageMatrix.UpdateLayout();
            for(int colIndex = 0; colIndex < width; colIndex++) {
                for(int rowIndex = 0; rowIndex < height; rowIndex++) {
                    int value = matrix[rowIndex * width + colIndex];
                    colIndex++;
                    if(value == 1) {
                        ColorCell(rowIndex, colIndex, Color.FromRgb(120, 120, 120));
                    }
                    else if (value == 3) {
                        ColorCell(rowIndex, colIndex, Color.FromRgb(10, 240, 10));
                    }
                    else if (value == 4) {
                        ColorCell(rowIndex, colIndex, Color.FromRgb(10, 190, 10));
                    }
                    else if (value == 5) {
                        ColorCell(rowIndex, colIndex, Color.FromRgb(10, 145, 10));
                    }
                    else if (value > 5) {
                        ColorCell(rowIndex, colIndex, Color.FromRgb(10, 100, 10));
                    }
                    colIndex--;
                }
            }
        }

        private void ColorCell(int rowInd, int colInd, Color color) {
            DataGridCell cell = GetCell(rowInd, colInd);
            if (cell == null) {
                return;
            }
            cell.Background = new SolidColorBrush(color);
        }

        public void DisplayMatrix<T>(T[] matrix, int width, int height,
                                                 string[] rowNames, string[] columnNames) {
            imageMatrix.Columns.Clear();
            imageMatrix.Items.Clear();


            try {
                this.width = width;
                this.matrix = new int[matrix.Length];
                for (int i = 0; i < matrix.Length; i++) {
                    this.matrix[i] = int.Parse(matrix[i].ToString());
                }
            }
            catch (Exception e) {
                this.matrix = null;
            }

            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "";
            col1.Binding = new Binding("-1");

            imageMatrix.Columns.Add(col1);
            for (int x = 0; x < width; x++) {
                DataGridTextColumn col = new DataGridTextColumn();
                col.Header = columnNames[x].ToString();
                col.Binding = new Binding(x.ToString());

                imageMatrix.Columns.Add(col);
            }

            for (int y = 0; y < height; y++) {
                dynamic row = new ExpandoObject();
                ((IDictionary<string, object>)row)["-1"] = rowNames[y];

                for (int x = 0; x < width; x++) {
                    ((IDictionary<string, object>)row)[x.ToString()] = matrix[y * width + x].ToString();
                }
                imageMatrix.Items.Add(row);
            }
        }

        public void ApplyChanges(ImageData img) {
            int width = img.Width, height = img.Height;

            int i = 0;
            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {
                    img.SetPixel(x, y, matrix[i++]);
                }
            }
            img.ApplyChanges();
        }


        /// <param name="mask">for R    mask = 0x00FFFF</param>
        /// <param name="shift">for R    shift = 16</param>
        public void ApplyChangesInChannel(ImageData img, int mask, int shift) {
            int width = img.Width, height = img.Height;

            int i = 0;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    int pxlValue = img.GetPixel(x, y);
                    pxlValue = (pxlValue & mask) + (matrix[i++] << shift);
                    img.SetPixel(x, y, pxlValue);
                }
            }
            img.ApplyChanges();
        }
    }
}
