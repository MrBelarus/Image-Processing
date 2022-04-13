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
    /// Логика взаимодействия для InputValueDialog.xaml
    /// </summary>
    public partial class InputValueWindow : Window {
        public double Value { get; private set; }

        public bool doApply = false;

        public InputValueWindow() {
            InitializeComponent();
        }

        private void Continue_Click(object sender, RoutedEventArgs e) {
            try {
                Value = Convert.ToDouble(inputValue.Text);
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
