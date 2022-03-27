using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace ImageProcessing.Utils {
    public class UI_Utility {
        public static void InitComboBoxMenu(ComboBox comboBox, string[] items) {
            foreach (string item in items) {
                comboBox.Items.Add(item);
            }
            comboBox.SelectedIndex = 0;
        }
    }
}
