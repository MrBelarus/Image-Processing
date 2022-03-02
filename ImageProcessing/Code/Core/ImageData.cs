using ImageProcessing.Utils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace ImageProcessing.Code.Core {
    public class ImageData {
        private Bitmap _bitmap;

        public ImageData(Bitmap bitmap) {
            _bitmap = bitmap;
        }

        public ImageData(string pathToImage) {
            _bitmap = ImageUtility.LoadImage(pathToImage);
        }

        public int Width => _bitmap.Width;
        public int Height => _bitmap.Height;

        public int ColorDepth {
            get {
                if (_bitmap == null) {
                    throw new Exception("Can't get color depth - bitmap was null!");
                }
                return Image.GetPixelFormatSize(_bitmap.PixelFormat);
            }
        }

        private byte[] imgBytes = null;
        public byte[] ImageBytes {
            get {
                if (imgBytes == null || imgBytes.Length == 0) {
                    RefreshImageData(_bitmap.PixelFormat);
                }
                return imgBytes;
            }
        }

        private int stride = 0;
        public int Stride {
            get {
                if (imgBytes == null || imgBytes.Length == 0) {
                    RefreshImageData(_bitmap.PixelFormat);
                }
                return stride;
            }
        }

        public void RefreshImageData(PixelFormat format = PixelFormat.Format32bppArgb) {
            Tuple<byte[], int> result = ImageUtility.BitmapToBytes(_bitmap, format);
            imgBytes = result.Item1;
            stride = result.Item2;
        }

        public BitmapImage GetBitmapImage() {
            return ImageUtility.BitmapToImageSource(_bitmap);
        }

        public int GetPixel(int x, int y) {
            if (_bitmap == null) {
                throw new Exception("Bitmap is null, can't get pixel!");
            }

            int offsetY = y * Stride;
            int offsetX;
            int bitShift;

            switch (ColorDepth) {
                case 1:
                    offsetX = x / 8;
                    bitShift = 7 - (x % 8);
                    return (imgBytes[offsetY + offsetX] & GetPowerOfTwo(bitShift)) >> bitShift;
                case 4:
                    offsetX = x / 4;
                    bitShift = 3 - (x % 4);
                    return (imgBytes[offsetY + offsetX] & GetPowerOfTwo(bitShift)) >> bitShift;
                case 8:

                    break;
                case 16:

                    break;
                case 24:

                    break;
                case 32:

                    break;
            }

            return 1;
        }

        public int[] GetPixels() {
            int[] pixels = new int[Width * Height];

            for(int y = 0; y < Height; y++) {
                for(int x = 0; x < Width; x++) {
                    pixels[y * Width + x] = GetPixel(x, y);
                }
            }

            return pixels;
        }

        public void SetPixel(int x, int y, int value) {
            if (_bitmap == null) {
                throw new Exception("Bitmap is null, can't set pixel!");
            }

            int offsetY = y * Stride;
            int offsetX;
            byte newValue;

            switch (ColorDepth) {
                case 1:
                    offsetX = x / 8;
                    newValue = imgBytes[offsetY + offsetX];
                    if (value == 1) {
                        newValue |= (byte)((byte)value << (7 - x % 8));
                    }
                    else {
                        value = 1;
                        newValue &= (byte)~((byte)value << (7 - x % 8));
                        //newValue -= (byte)GetPowerOfTwo(7 - x % 8);
                    }
                    imgBytes[offsetY + offsetX] = newValue;
                    break;
                case 4:
                    offsetX = x / 4;
                    newValue = imgBytes[offsetY + offsetX];
                    if (value > 0) {
                        newValue |= (byte)value;
                    }
                    else {
                        value = 1;
                        newValue &= (byte)~((byte)value << (3 - x % 4));
                        //newValue -= (byte)GetPowerOfTwo(7 - x % 8);
                    }
                    imgBytes[offsetY + offsetX] = newValue;
                    break;
                case 8:

                    break;
                case 16:

                    break;
                case 24:

                    break;
                case 32:

                    break;
            }
        }

        public void ApplyChanges(PixelFormat format) {
            _bitmap = ImageUtility.BitmapFromBytes(imgBytes, Width, Height, format);
        }
        public void ApplyChanges() {
            _bitmap = ImageUtility.BitmapFromBytes(imgBytes, Width, Height, _bitmap.PixelFormat);
        }
        public void DiscardChanges(PixelFormat format) {
            RefreshImageData(format);
        }
        public void DiscardChanges() {
            RefreshImageData(_bitmap.PixelFormat);
        }

        private int GetPowerOfTwo(int power) {
            return 1 << power;
        }
    }
}
