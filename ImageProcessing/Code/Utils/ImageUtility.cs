using ImageProcessing.Core;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace ImageProcessing.Utils {
    class ImageUtility {
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

        /// <summary>
        /// Convert binary image to grey 24bit (manhattan distance)
        /// </summary>
        /// <param name="original">image to convert</param>
        /// <returns>New instance of ImageData with 24bit grayscale</returns>
        public static ImageData Convert1BitToGray24Bit(ImageData original) {
            int[] pixelValues = ImageMatrixCalculator.ConvertDistanceToPixelGreyValues(
                                ImageMatrixCalculator.GetManhattanDistanceMatrix(original));

            Bitmap bitmap =
                new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb);
            ImageData data = new ImageData(bitmap);
            for (int y = 0; y < data.Height; y++) {
                for (int x = 0; x < data.Width; x++) {
                    int pxl = pixelValues[y * data.Width + x];
                    data.SetPixel(x, y, (pxl << 16) + (pxl << 8) + pxl);
                }
            }
            data.ApplyChanges();

            return data;
        }

        public static ImageData Get1BitImageEmpty(ImageData source) {
            Bitmap _1bitmap = new Bitmap(source.Width, source.Height, PixelFormat.Format1bppIndexed);
            return new ImageData(_1bitmap);
        }

        //public static ImageData Convert24BitTo8BitGrey(ImageData original) {
        //    int[] pixelValues = original.GetPixels();
        //    Bitmap bitmap =
        //        new Bitmap(original.Width, original.Height, PixelFormat.Format8bppIndexed);
        //    ImageData data = new ImageData(bitmap);
        //    for (int y = 0; y < data.Height; y++) {
        //        for (int x = 0; x < data.Width; x++) {
        //            int pxl = pixelValues[y * data.Width + x];
        //            data.SetPixel(x, y, pxl & 0x000000ff);
        //        }
        //    }
        //    data.ApplyChanges();

        //    return data;
        //}

        /// <summary>
        /// Convert image to binary
        /// <paramref name="threshold">0-255 value</paramref>
        /// </summary>
        public static ImageData ConvertToBinary(ImageData original, bool ignoreAlpha, int threshold = 128) {
            Bitmap newBitMap = new Bitmap(original.Width, original.Height,
                PixelFormat.Format1bppIndexed);
            ImageData result = new ImageData(newBitMap);
            int width = original.Width;
            int clrDepth = original.ColorDepth;

            if (ignoreAlpha) {
                clrDepth = MathModule.Clamp(original.ColorDepth, 1, 24);
            }

            int maxPixelValue = GetMaxPixelValue(clrDepth);
            int t = (int)(maxPixelValue * ((float)threshold / 255));

            int[] oldPixels = original.GetPixels();

            if (ignoreAlpha) {
                int pxlValue;
                for (int i = 0; i < oldPixels.Length; i++) {
                    pxlValue = oldPixels[i] & 0x00ffffff;
                    result.SetPixel(i % width, i / width, pxlValue > t ? 1 : 0);
                }
            }
            else {
                for (int i = 0; i < oldPixels.Length; i++) {
                    result.SetPixel(i % width, i / width, oldPixels[i] > t ? 1 : 0);
                }
            }

            result.ApplyChanges();

            return result;
        }

        public static int GetMaxPixelValue(int clrDepth) {
            switch (clrDepth) {
                case 1:
                    return 1;
                case 4:
                    return 15;
                case 8:
                    return 255;
                case 16:
                    return 65_535;
                case 24:
                    return 16_777_215;
                case 32:
                    return 2_147_483_647;
            }
            return int.MaxValue;
        }
    }
}
