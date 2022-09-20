using ImageProcessing.Core;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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

            string[] labels = new string[width];
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
