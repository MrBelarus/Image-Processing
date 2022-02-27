using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ImageProcessing.Utils {
    class ImageManager {
        public static BitmapImage LoadImage(string path) {
            try {
                return new BitmapImage(new Uri(path));
            }
            catch {
                return null;
            }
        }

        public static void SaveImage(Image image, string path, BitmapEncoder encoder = null) {
            if (encoder == null) {
                encoder = new PngBitmapEncoder();
            }
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
            using (FileStream stream = new FileStream(path, FileMode.Create)) {
                encoder.Save(stream);
            }
        }
    }
}
