
using ImageProcessing.Core;
using ImageProcessing.Utils;
using System;
using System.Drawing;

namespace ImageProcessing.Core.Processers {
    public class InvertColor : ImageProcesser {
        public override ImageData Process(ImageData image) {
            int maxPixelValue;
            switch (image.ColorDepth) {
                case 1:
                    maxPixelValue = 1;
                    break;
                case 4:
                    maxPixelValue = 15;
                    break;
                case 8:
                    maxPixelValue = 255;
                    break;
                default:
                    throw new Exception("Not supported image format");
            }

            for (int y = 0; y < image.Height; y++) {
                for (int x = 0; x < image.Width; x++) {
                    int pixel = image.GetPixel(x, y);
                    image.SetPixel(x, y, maxPixelValue - pixel);
                }
            }
            image.ApplyChanges();

            return image;
        }
    }
}
