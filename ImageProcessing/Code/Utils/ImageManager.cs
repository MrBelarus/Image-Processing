using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace ImageProcessing.Utils {
    class ImageManager {
        public static Bitmap LoadImage(string path) {
            try {
                return new Bitmap(path);
            }
            catch {
                return null;
            }
        }

        public static BitmapImage BitmapToImageSource(Bitmap bitmap) {
            using (MemoryStream memory = new MemoryStream()) {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public static void SaveImage(System.Windows.Controls.Image image, string path, BitmapEncoder encoder = null) {
            if (encoder == null) {
                encoder = new PngBitmapEncoder();
            }
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
            using (FileStream stream = new FileStream(path, FileMode.Create)) {
                encoder.Save(stream);
            }
        }

        public static void SaveImage(Bitmap bitmap, string path, ImageFormat imageFormat) {
            bitmap.Save(path, imageFormat);
        }

        /// <returns>item1 - imgBytes, item2 - stride value</returns>
        public static Tuple<byte[], int> BitmapToBytes(Bitmap bmp, PixelFormat format = PixelFormat.Format32bppArgb) {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, format);
            int byteCount = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] bytes = new byte[byteCount];
            Marshal.Copy(bmpData.Scan0, bytes, 0, byteCount);
            bmp.UnlockBits(bmpData);
            return new Tuple<byte[], int>(bytes, bmpData.Stride);
        }

        public static Bitmap BitmapFromBytes(byte[] bytes, int width, int height, PixelFormat bmpFormat) {
            Bitmap bmp = new Bitmap(width, height, bmpFormat);
            var rect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmpFormat);
            Marshal.Copy(bytes, 0, bmpData.Scan0, bytes.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        /// <summary>
        /// How many bits responsible for color
        /// </summary>
        public static int GetColorDepth(Bitmap bitmap) {
            return Image.GetPixelFormatSize(bitmap.PixelFormat);
        }
    }
}
