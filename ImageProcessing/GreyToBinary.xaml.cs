using ImageProcessing.Utils;
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
    /// Interaction logic for GreyToBinary.xaml
    /// </summary>
    public partial class GreyToBinarySettingsWindow : Window {
        public int Threshold { get; private set; }
        public bool IgnoreAlpha { get; private set; }

        public GreyToBinarySettingsWindow() {
            InitializeComponent();
        }

        private void Continue_Click(object sender, RoutedEventArgs e) {
            try {
                Threshold = MathModule.Clamp(Convert.ToInt32(thresholdValue.Text), 0, 255);
                IgnoreAlpha = (bool)ignoreAlpha.IsChecked;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.DialogResult = true;
        }
    }
}
