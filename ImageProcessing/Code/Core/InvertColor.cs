using ImageProcessing.Core;
using ImageProcessing.Utils;
using System;
using System.Drawing;

namespace ImageProcessing.Code.Core {
    class InvertColor : ImageProcesser {

        // Access pixel at (x,y) [RGBA32]
        //B = bytes[bitmapData.Scan0 + x * 4 + y * bitmapData.Stride + 0]
        //G = bytes[bitmapData.Scan0 + x * 4 + y * bitmapData.Stride + 1]
        //R = bytes[bitmapData.Scan0 + x * 4 + y * bitmapData.Stride + 2]
        //A = bytes[bitmapData.Scan0 + x * 4 + y * bitmapData.Stride + 3]

        // Access pixel at (x,y) [RGBA24]
        //B = bytes[bitmapData.Scan0 + x * 4 + y * bitmapData.Stride + 0]
        //G = bytes[bitmapData.Scan0 + x * 4 + y * bitmapData.Stride + 1]
        //R = bytes[bitmapData.Scan0 + x * 4 + y * bitmapData.Stride + 2]

        //https://stackoverflow.com/questions/51071944/how-can-i-work-with-1-bit-and-4-bit-images

        public override Bitmap Process(Bitmap bitmap) {
            int colorDepth = ImageManager.GetColorDepth(bitmap);
            Tuple<byte[], int> imgBytesData = ImageManager.BitmapToBytes(bitmap, bitmap.PixelFormat);

            switch (colorDepth) {
                case 1:
                    _1BitColorDepth(ref imgBytesData, bitmap);
                    break;
                case 4:
                    //
                    break;
                case 8:
                    //
                    break;
                case 16:
                    //
                    break;
                case 24:
                    //
                    break;
                case 32:
                    //
                    break;
                default:
                    throw new Exception("Not supported image format");
            }

            return ImageManager.BitmapFromBytes(imgBytesData.Item1, bitmap.Width, bitmap.Height, bitmap.PixelFormat);
        }

        private void _1BitColorDepth(ref Tuple<byte[], int> imgData, Bitmap bitmap) {
            //read data
            int[] pixelValues = new int[bitmap.Width * bitmap.Height];
            for (int y = 0; y < bitmap.Height; y++) {
                int offsetY = y * imgData.Item2;
                for (int x = 0; x < bitmap.Width; x++) {
                    int offsetX = x / 8;
                    int bitShift = 7 - (x % 8);

                    pixelValues[y * bitmap.Width + x] = (imgData.Item1[offsetY + offsetX] & (byte)Math.Pow(2, bitShift)) >> bitShift;
                }
            }

            //invert logic
            for (int i = 0; i < pixelValues.Length; i++) {
                pixelValues[i] = pixelValues[i] == 0 ? 1 : 0;
            }

            //set data
            for (int y = 0; y < bitmap.Height; y++) {
                int offsetY = y * imgData.Item2;
                for (int x = 0; x < bitmap.Width; x += 8) {
                    int offsetX = x / 8;

                    byte newValue = 0b00000000;
                    for (int bit = 0; bit < 8 && bit < bitmap.Width - x; bit++) {
                        newValue |= (byte)((byte)pixelValues[y * bitmap.Width + x + bit] << (7 - bit));
                    }

                    imgData.Item1[offsetY + offsetX] = newValue;
                }
            }
        }
    }
}
