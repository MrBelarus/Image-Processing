using ImageProcessing.Core;
using ImageProcessing.Core.Processers;
using ImageProcessing.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Core.Processers {
    public class ConvertToBinaryS9 : ImageProcesser {
        public override ImageData Process(ImageData image) {
            //if (image.ColorDepth > 8) {
            //    image = ImageUtility.Convert24BitTo8BitGrey(image);
            //}
            if (image.ColorDepth == 32) {
                //null alpha
                for (int y = 0; y < image.Height; y++) {
                    for (int x = 0; x < image.Width; x++) {
                        int pixel = image.GetPixel(x, y);
                        image.SetPixel(x, y, pixel & 0x00ffffff);
                    }
                }
            }
            image.ApplyChanges();
            int[] pxls = image.GetPixels();
            int[] a8_matrix = ImageMatrixCalculator.GetA8Matrix(image);
            int imgWidth = image.Width;

            for (int y = 0; y < image.Height; y++) {
                for (int x = 0; x < imgWidth; x++) {
                    int i = y * imgWidth + x;
                    int pixel = pxls[i];
                    image.SetPixel(x, y,
                        ((a8_matrix[i] + pixel) / 9) < pixel ? 0x00ffffff : 0);
                }
            }
            image.ApplyChanges();

            return image;
        }
    }
}
