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
    /// Логика взаимодействия для Matrix3x3Input.xaml
    /// </summary>
    public partial class Matrix3x3Input : Window {
        public double[][] Matrix3x3 { get; private set; }

        public int[][] Matrix3x3Int {
            get {
                if (Matrix3x3 == null) {
                    return null;
                }
                int[][] result = new int[3][];
                for (int i = 0; i < 3; i++) {
                    result[i] = new int[3];
                    for(int j = 0; j < 3; j++) {
                        result[i][j] = (int)Matrix3x3[i][j];
                    }
                }

                return result;
            }
        }

        public bool doApply = false;

        public Matrix3x3Input() {
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e) {
            try {
                double el00 = Convert.ToDouble(el_00.Text);
                double el01 = Convert.ToDouble(el_01.Text);
                double el02 = Convert.ToDouble(el_02.Text);
                double el10 = Convert.ToDouble(el_10.Text);
                double el11 = Convert.ToDouble(el_11.Text);
                double el12 = Convert.ToDouble(el_12.Text);
                double el20 = Convert.ToDouble(el_20.Text);
                double el21 = Convert.ToDouble(el_21.Text);
                double el22 = Convert.ToDouble(el_22.Text);

                Matrix3x3 = new double[3][] {
                    new double[] {el00, el01, el02},
                    new double[] {el10, el11, el12},
                    new double[] {el20, el21, el22},
                };
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            doApply = true;
            this.DialogResult = true;
        }
    }
}
