﻿using ImageProcessing.Utils;

namespace ImageProcessing.Core.Processers {
    public class ConvertToBinaryThreshold : ImageProcesser {
        private int _threshold = 128;
        private bool _ignoreAlpha = true;

        public ConvertToBinaryThreshold() {
            GreyToBinarySettingsWindow settingsWindow = new GreyToBinarySettingsWindow();
            settingsWindow.ShowDialog();

            if (settingsWindow.DialogResult == true) {
                _threshold = settingsWindow.Threshold;
                _ignoreAlpha = settingsWindow.IgnoreAlpha;
            }
        }

        public override ImageData Process(ImageData image) {
            image = ImageUtility.ConvertToBinary(image, _ignoreAlpha, _threshold);
            return image;
        }
    }
}