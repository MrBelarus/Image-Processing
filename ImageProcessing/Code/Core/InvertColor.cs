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

        //var watch = System.Diagnostics.Stopwatch.StartNew();
        //// the code that you want to measure comes here
        //watch.Stop();

        //https://stackoverflow.com/questions/51071944/how-can-i-work-with-1-bit-and-4-bit-images

        public override ImageData Process(ImageData image) {
            switch (image.ColorDepth) {
                case 1:
                    _1BitColorDepth(image);
                    break;
                case 4:
                    _4BitColorDepth(image);
                    break;
                case 8:
                    _8BitColorDepth(image);
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

            return image;
        }

        private void _1BitColorDepth(ImageData imageData) {
            int maxPixelValue = 1;
            for (int y = 0; y < imageData.Height; y++) {
                for (int x = 0; x < imageData.Width; x++) {
                    int pixel = imageData.GetPixel(x, y);
                    imageData.SetPixel(x, y, maxPixelValue - pixel);
                }
            }
            imageData.ApplyChanges();
        }

        private void _4BitColorDepth(ImageData imageData) {
            int maxPixelValue = 15;
            for (int y = 0; y < imageData.Height; y++) {
                for (int x = 0; x < imageData.Width; x++) {
                    int pixel = imageData.GetPixel(x, y);
                    imageData.SetPixel(x, y, maxPixelValue - pixel);
                }
            }
            imageData.ApplyChanges();
        }

        private void _8BitColorDepth(ImageData imageData) {
            int maxPixelValue = 255;
            for (int y = 0; y < imageData.Height; y++) {
                for (int x = 0; x < imageData.Width; x++) {
                    int pixel = imageData.GetPixel(x, y);
                    imageData.SetPixel(x, y, maxPixelValue - pixel);
                }
            }
            imageData.ApplyChanges();
        }
    }
}
