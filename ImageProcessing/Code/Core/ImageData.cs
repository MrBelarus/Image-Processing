using ImageProcessing.Utils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace ImageProcessing.Core {
    public class ImageData {
        private Bitmap _bitmap;
        public bool IsImageLoaded => _bitmap != null;

        public ImageData(Bitmap bitmap) {
            _bitmap = bitmap;
        }

        public ImageData(string pathToImage) {
            _bitmap = ImageUtility.LoadImage(pathToImage);
        }

        public ImageData(ImageData from) {
            this._bitmap = ImageUtility.BitmapFromBytes(
                from.ImageBytes, from.Width, from.Height, from._bitmap.PixelFormat, from._bitmap.Palette);
        }

        public int Width => _bitmap.Width;
        public int Height => _bitmap.Height;
        public PixelFormat PixelFormat => _bitmap.PixelFormat;

        private int clrDepth;
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
            clrDepth = ColorDepth;
            var palette = _bitmap.Palette;
        }

        public BitmapImage GetBitmapImage() {
            return ImageUtility.BitmapToImageSource(_bitmap);
        }

        /// <summary>
        /// Get pixel value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>pixel value [RGB/ARGB if 24/32bit]</returns>
        public int GetPixel(int x, int y) {
            if (_bitmap == null) {
                throw new Exception("Bitmap is null, can't get pixel!");
            }

            int offsetY = y * Stride;
            int offsetX;
            int bitShift;

            switch (clrDepth) {
                case 1:
                    offsetX = x / 8; //every bit is a new pixel, so 1 byte has 8 pixels
                    bitShift = 7 - (x % 8);
                    return (imgBytes[offsetY + offsetX] & GetPowerOfTwo(bitShift)) >> bitShift;
                case 4:
                    offsetX = x / 2; //every 4bits is a new pixel, so 1 byte has 2 pixels
                    if (x % 2 == 0) {
                        return imgBytes[offsetY + offsetX] >> 4;
                    }
                    else {
                        return imgBytes[offsetY + offsetX] & (0x0f);
                    }
                case 8:
                    offsetX = x; //every 8bits is a new pixel, so 1 byte has 1 pixels
                    var val = imgBytes[offsetY + offsetX];
                    return val;
                case 16:
                    offsetX = 2 * x;
                    if (BitConverter.IsLittleEndian) {
                        return (imgBytes[offsetY + offsetX + 1] << 8) + imgBytes[offsetY + offsetX];
                    }
                    return (imgBytes[offsetY + offsetX] << 8) + imgBytes[offsetY + offsetX + 1];
                case 24:
                    offsetX = 3 * x;
                    if (BitConverter.IsLittleEndian) {
                        return (imgBytes[offsetY + offsetX + 2] << 16) +
                           (imgBytes[offsetY + offsetX + 1] << 8) + imgBytes[offsetY + offsetX];
                    }
                    return (imgBytes[offsetY + offsetX] << 16) + (imgBytes[offsetY + offsetX + 1] << 8) +
                           imgBytes[offsetY + offsetX + 2];
                case 32:
                    offsetX = 4 * x;
                    if (BitConverter.IsLittleEndian) {
                        return (imgBytes[offsetY + offsetX + 3] << 24) + (imgBytes[offsetY + offsetX + 2] << 16) +
                           (imgBytes[offsetY + offsetX + 1] << 8) + imgBytes[offsetY + offsetX];
                    }
                    return (imgBytes[offsetY + offsetX] << 24) + (imgBytes[offsetY + offsetX + 1] << 16) +
                           (imgBytes[offsetY + offsetX + 2] << 8) + imgBytes[offsetY + offsetX + 3];
            }

            return 1;
        }

        /// <summary>
        /// Get pixel values
        /// </summary>
        /// <returns>pixel values [RGB/ARGB if 24/32bit]</returns>
        public int[] GetPixels() {
            int[] pixels = new int[Width * Height];

            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    pixels[y * Width + x] = GetPixel(x, y);
                }
            }

            return pixels;
        }

        public int[] GetBinaryPixelsInverted() {
            int[] pixels = new int[Width * Height];

            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    pixels[y * Width + x] = GetPixel(x, y) == 1 ? 0 : 1;
                }
            }

            return pixels;
        }

        public string[] GetPixelsRGB() {
            string[] pixels = new string[Width * Height];

            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    int pxl = GetPixel(x, y);
                    //ARGB
                    pixels[y * Width + x] = ((pxl & 0x00ff0000) >> 16).ToString() + ", " +
                                            ((pxl & 0x0000ff00) >> 8).ToString() + ", " + (pxl & 0x000000ff).ToString();
                }
            }

            return pixels;
        }

        /// <param name="mask">R - mask=0xff0000, rightShift=16</param>
        public string[] GetChannelValuesString(int mask, int rightShift) {
            string[] pixels = new string[Width * Height];
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    int pxl = GetPixel(x, y);
                    //ARGB
                    pixels[y * Width + x] = ((pxl & mask) >> rightShift).ToString();
                }
            }

            return pixels;
        }

        /// <param name="mask">R - mask=0xff0000, rightShift=16</param>
        public int[] GetChannelValues(int mask, int rightShift) {
            int[] channel = new int[Width * Height];
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    int pxl = GetPixel(x, y);
                    //ARGB
                    channel[y * Width + x] = ((pxl & mask) >> rightShift);
                }
            }
            return channel;
        }

        /// <summary>
        /// Set pixel value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value">if 24/32bit format is RGB/ARGB</param>
        public void SetPixel(int x, int y, int value) {
            if (_bitmap == null) {
                throw new Exception("Bitmap is null, can't set pixel!");
            }

            int offsetY = y * Stride;
            int offsetX;
            byte imgByte;

            switch (clrDepth) {
                case 1:
                    offsetX = x / 8; //every bit is a new pixel, so 1 byte has 8 pixels
                    value = Math.Clamp(value, 0, 1);
                    imgByte = imgBytes[offsetY + offsetX];
                    if (value == 1) {
                        imgByte |= (byte)((byte)value << (7 - x % 8));
                    }
                    else {
                        value = 1;
                        imgByte &= (byte)~((byte)value << (7 - x % 8));
                    }
                    imgBytes[offsetY + offsetX] = imgByte;
                    break;
                case 4:
                    offsetX = x / 2; //every 4bits is a new pixel, so 1 byte has 2 pixels
                    value = Math.Clamp(value, 0, 255);
                    if (x % 2 == 0) {
                        imgBytes[offsetY + offsetX] = (byte)(imgBytes[offsetY + offsetX] & 0x0f | (value << 4));
                    }
                    else {
                        imgBytes[offsetY + offsetX] = (byte)(imgBytes[offsetY + offsetX] & 0xf0 | value);
                    }
                    break;
                case 8:
                    offsetX = x; //every 8bits is a new pixel, so 1 byte has 1 pixels
                    imgBytes[offsetY + offsetX] = (byte)Math.Clamp(value, 0, 255);
                    break;
                case 16:
                    offsetX = 2 * x;
                    if (BitConverter.IsLittleEndian) {
                        imgBytes[offsetY + offsetX] = (byte)(value & 0xff);
                        imgBytes[offsetY + offsetX + 1] = (byte)(value >> 8);
                    }
                    else {
                        imgBytes[offsetY + offsetX] = (byte)(value >> 8);
                        imgBytes[offsetY + offsetX + 1] = (byte)(value & 0xff);
                    }
                    break;
                case 24:
                    offsetX = 3 * x;
                    if (BitConverter.IsLittleEndian) {
                        imgBytes[offsetY + offsetX] = (byte)(value & 0x0000ff);
                        imgBytes[offsetY + offsetX + 1] = (byte)((value & 0x00ff00) >> 8);
                        imgBytes[offsetY + offsetX + 2] = (byte)((value & 0xff0000) >> 16);
                    }
                    else {
                        imgBytes[offsetY + offsetX] = (byte)((value & 0xff0000) >> 16);
                        imgBytes[offsetY + offsetX + 1] = (byte)((value & 0x00ff00) >> 8);
                        imgBytes[offsetY + offsetX + 2] = (byte)(value & 0x0000ff);
                    }
                    break;
                case 32:
                    offsetX = 4 * x;
                    if (BitConverter.IsLittleEndian) {
                        imgBytes[offsetY + offsetX] = (byte)(value & 0x000000ff);
                        imgBytes[offsetY + offsetX + 1] = (byte)((value & 0x0000ff00) >> 8);
                        imgBytes[offsetY + offsetX + 2] = (byte)((value & 0x00ff0000) >> 16);
                        imgBytes[offsetY + offsetX + 3] = (byte)((value & 0xff000000) >> 24);
                    }
                    else {
                        imgBytes[offsetY + offsetX] = (byte)((value & 0xff000000) >> 24);
                        imgBytes[offsetY + offsetX + 1] = (byte)((value & 0x00ff0000) >> 16);
                        imgBytes[offsetY + offsetX + 2] = (byte)((value & 0x0000ff00) >> 8);
                        imgBytes[offsetY + offsetX + 3] = (byte)(value & 0x000000ff);
                    }
                    break;
            }
        }

        public void SetPixels(int[] pixelsData) {
            for(int y =0; y < Height; y++) {
                for(int x = 0; x < Width; x++) {
                    SetPixel(x, y, pixelsData[y * Width + x]);
                }
            }
        }

        /// <param name="value">[0..255]</param>
        public void SetPixelGrey(int x, int y, int value) {
            value = MathModule.Clamp(value, 0, 255);
            switch (clrDepth) {
                case 8:
                    SetPixel(x, y, value);
                    break;
                default:
                    SetPixel(x, y, value + (value << 8) + (value << 16));
                    break;
            }
        }

        public void SetPixel(int x, int y, Color color) {
            switch (clrDepth) {
                case 1:
                    //(depends on LUT!)
                    SetPixel(x, y, color == Color.Black ? 0 : 1);
                    break;
                case 4:
                    //(depends on LUT!)
                    break;
                case 8:
                    //(depends on LUT!)
                    //RRRGGGBB in 8bit below
                    int R = Math.Clamp((int)color.R, 0, 7); // 7 = 0b111
                    int G = Math.Clamp((int)color.G, 0, 7); //
                    int B = Math.Clamp((int)color.B, 0, 3); // 3 = 0b11
                    SetPixel(x, y, (R << 5) + (G << 2) + B);
                    break;
                default:
                    //for 16/24/32 bit should work
                    _bitmap.SetPixel(x, y, color);
                    break;
            }
        }

        public Color GetColor(int x, int y) {
            switch (clrDepth) {
                case 1:
                    //(depends on LUT!)
                    return GetPixel(x, y) == 0 ? Color.Black : Color.White;
                case 4:
                    //(depends on LUT! Results may be not as expected)
                    //TODO: 4bit support
                    return _bitmap.GetPixel(x, y);
                case 8:
                    //(depends on LUT! Results may be not as expected)
                    //RRRGGGBB in 8bit
                    int pxlValue = GetPixel(x, y);
                    int R = pxlValue >> 5;
                    int G = (pxlValue & 0b0001111) >> 2;
                    int B = pxlValue & 0b0000011;
                    return Color.FromArgb(R, G, B);
                case 16:
                    if (_bitmap.PixelFormat != PixelFormat.Format16bppRgb565) {
                        return _bitmap.GetPixel(x, y);
                    }
                    //16bit rgb565
                    pxlValue = GetPixel(x, y);
                    R = pxlValue >> 11;
                    G = (pxlValue & 0x000007E0) >> 5;
                    B = pxlValue & 0x0000001F;
                    return Color.FromArgb(R, G, B);
                case 24:
                    pxlValue = GetPixel(x, y);
                    R = (pxlValue & 0x00ff0000) >> 16;
                    G = (pxlValue & 0x0000ff00) >> 8;
                    B = pxlValue & 0x000000ff;
                    return Color.FromArgb(R, G, B);
                case 32:
                    if (_bitmap.PixelFormat != PixelFormat.Format32bppArgb) {
                        return _bitmap.GetPixel(x, y);
                    }
                    pxlValue = GetPixel(x, y);
                    int A = (int)(pxlValue & 0xff000000) >> 24;
                    R = (pxlValue & 0x00ff0000) >> 16;
                    G = (pxlValue & 0x0000ff00) >> 8;
                    B = pxlValue & 0x000000ff;
                    return Color.FromArgb(A, R, G, B);
            }

            return default;
        }

        public int GetBrightness(int x, int y) {
            return (int)(_bitmap.GetPixel(x, y).GetBrightness() * 255);
        }

        public int[] GetBrightnessArray() {
            int[] result = new int[Width * Height];
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    result[y * Width + x] = GetBrightness(x, y);
                }
            }
            return result;
        }

        public void ApplyChanges(PixelFormat format) {
            //TODO: test format
            _bitmap = ImageUtility.BitmapFromBytes(ImageBytes, Width, Height, format, _bitmap.Palette);
        }
        public void ApplyChanges() {
            ApplyChanges(_bitmap.PixelFormat);
        }
        public void DiscardChanges(PixelFormat format) {
            //TODO: test format
            RefreshImageData(format);
        }
        public void DiscardChanges() {
            DiscardChanges(_bitmap.PixelFormat);
        }

        private int GetPowerOfTwo(int power) {
            return 1 << power;
        }
    }
}
