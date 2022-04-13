using ImageProcessing.Utils;

namespace ImageProcessing.Core.Processers {
    public class ConvertToBinaryThreshold : ImageProcesser {
        private int _threshold = 128;
        private bool _ignoreAlpha = true;


        public override ImageData Process(ImageData image) {
            GreyToBinarySettingsWindow settingsWindow = new GreyToBinarySettingsWindow();
            settingsWindow.ShowDialog();

            if (settingsWindow.doApply == false) {
                return null;
            }

            _threshold = settingsWindow.Threshold;
            _ignoreAlpha = settingsWindow.IgnoreAlpha;
            image = ImageUtility.ConvertToBinary(image, _ignoreAlpha, _threshold);
            return image;
        }
    }
}
