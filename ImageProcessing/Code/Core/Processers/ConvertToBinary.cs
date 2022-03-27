using ImageProcessing.Utils;

namespace ImageProcessing.Core.Processers {
    public class ConvertToBinary : ImageProcesser {
        private int _threshold = 128;
        private bool _ignoreAlpha = true;

        public ConvertToBinary() {
            GreyToBinarySettingsWindow settingsWindow = new GreyToBinarySettingsWindow();
            settingsWindow.ShowDialog();

            if (settingsWindow.DialogResult == true) {
                _threshold = settingsWindow.Threshold;
                _ignoreAlpha = settingsWindow.IgnoreAlpha;
            }
        }

        public override ImageData Process(ImageData image) {
            if (_threshold == 128) {
                image = ImageUtility.ConvertToBinary(image, _ignoreAlpha);
            }
            return image;
        }
    }
}
