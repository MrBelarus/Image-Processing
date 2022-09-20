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
    /// Логика взаимодействия для TexureComparatorWindow.xaml
    /// </summary>
    public partial class TexureComparatorWindow : Window {
        public TexureComparatorWindow() {
            InitializeComponent();
        }

        private void CalculateDistance(object sender, RoutedEventArgs e) {
            float energy1 = float.Parse(energyText.Text);
            float energy2 = float.Parse(energyText_2.Text);

            float homogen1 = float.Parse(homogenText.Text);
            float homogen2 = float.Parse(homogenText_2.Text);

            float contrast1 = float.Parse(contrastText.Text);
            float contrast2 = float.Parse(contrastText_2.Text);
            
            float entropy1 = float.Parse(entropyText.Text);
            float entropy2 = float.Parse(entropyText_2.Text);

            float result = (float)Math.Sqrt(Math.Pow((entropy2 - entropy1), 2) +
                         Math.Pow((energy1 - energy2), 2) +
                         Math.Pow((homogen1 - homogen2), 2));
                         //Math.Pow((contrast2 - contrast1), 2));
            tex1Result.Text = result.ToString();
        }
    }
}
