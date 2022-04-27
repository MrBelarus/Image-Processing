using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Core.Processers {
    public class AverageFiltration3x3 : ImageProcesser {
        int[][] _mask3x3;

        public AverageFiltration3x3(int[][] mask3x3) {
            _mask3x3 = mask3x3;
        }

        public override ImageData Process(ImageData image) {
            int width = image.Width, height = image.Height;
            int[] pixels = image.GetPixels();
            
            //convert pixels values to 0-255 blue channel values
            for(int i = 0; i < pixels.Length; i++) {
                pixels[i] = pixels[i] & 0xff;
            }

            //first and last strings
            int lowestStringStartInd = width * (height - 1);
            for (int x = 1; x < width - 1; x++) {
                float pxl = 
                    pixels[x] * _mask3x3[1][1] +
                    pixels[x - 1] * _mask3x3[0][1] + 
                    pixels[x + 1] * _mask3x3[2][1] + 
                    pixels[width + x] * _mask3x3[1][2] +
                    pixels[width + x - 1] * _mask3x3[0][2] + 
                    pixels[width + x + 1] * _mask3x3[2][2];
                image.SetPixelGrey(x, 0, (int) MathF.Round(pxl / 6f));

                pxl = 
                    pixels[lowestStringStartInd + x] * _mask3x3[1][1] +
                    pixels[lowestStringStartInd + x - 1] * _mask3x3[0][1] +
                    pixels[lowestStringStartInd + x + 1] * _mask3x3[2][1] + 
                    pixels[lowestStringStartInd + x - width] * _mask3x3[1][2] +
                    pixels[lowestStringStartInd - width + x - 1] * _mask3x3[0][2] + 
                    pixels[lowestStringStartInd - width + x + 1] * _mask3x3[2][2];

                image.SetPixelGrey(x, height - 1, (int)MathF.Round(pxl / 6f));
            }

            //first and last columns
            for (int y = 1; y < height - 1; y++) {
                float pxl = 
                    pixels[width * y] * _mask3x3[1][1] +
                    pixels[width * y + 1] * _mask3x3[2][1] +
                    pixels[width * (y - 1)] * _mask3x3[1][0] + 
                    pixels[width * (y + 1)] * _mask3x3[1][2] +
                    pixels[width * (y - 1) + 1] * _mask3x3[2][0] + 
                    pixels[width * (y + 1) + 1] * _mask3x3[2][2];
                image.SetPixelGrey(0, y, (int)MathF.Round(pxl / 6f));

                pxl = 
                    pixels[width * y - 1] * _mask3x3[1][0] + 
                    pixels[width * (y + 1) - 1] * _mask3x3[1][1] +
                    pixels[width * (y + 2) - 1] * _mask3x3[1][2] +
                    pixels[width * (y + 1) - 2] * _mask3x3[0][1] +
                    pixels[width * (y + 2) - 2] * _mask3x3[0][2] + 
                    pixels[width * y - 2] * _mask3x3[0][0];
                image.SetPixelGrey(width - 1, y, (int)MathF.Round(pxl / 6f));
            }

            
            //corners
            image.SetPixelGrey(0, 0, (int)MathF.Round((
                pixels[0] * _mask3x3[1][1] + 
                pixels[1] * _mask3x3[2][1] + 
                pixels[width] * _mask3x3[1][2] + 
                pixels[width + 1] * _mask3x3[2][2]) / 4f));
            image.SetPixelGrey(width - 1, 0, (int)MathF.Round((
                pixels[width - 1] * _mask3x3[1][1] + 
                pixels[width - 2] * _mask3x3[0][1] + 
                pixels[width * 2 - 1] * _mask3x3[1][2] + 
                pixels[width * 2 - 2] * _mask3x3[0][2]) / 4f));
            image.SetPixelGrey(0, height - 1, (int)MathF.Round((
                pixels[width * (height - 1)] * _mask3x3[1][1] + 
                pixels[width * (height - 2)] * _mask3x3[1][0] + 
                pixels[width * (height - 2) + 1] * _mask3x3[2][0] + 
                pixels[width * (height - 1) + 1] * _mask3x3[2][1]) / 4f));
            image.SetPixelGrey(width - 1, height - 1, (int)MathF.Round((
                pixels[width * height - 1] * _mask3x3[1][1] +
                pixels[width * height - 2] * _mask3x3[0][1] + 
                pixels[width * (height - 1) - 1] * _mask3x3[1][0] + 
                pixels[width * (height - 1) - 2] * _mask3x3[0][0]) / 4f));

            //main body
            for (int y = 1; y < height - 1; y++) {
                for (int x = 1; x < width - 1; x++) {
                    float pxl = 
                        pixels[y * width + x] * _mask3x3[1][1] +
                        pixels[(y - 1) * width + x] * _mask3x3[1][0] + 
                        pixels[(y + 1) * width + x] * _mask3x3[1][2] +
                        pixels[y * width + x - 1] * _mask3x3[0][1] + 
                        pixels[y * width + x + 1] * _mask3x3[2][1] +
                        pixels[(y + 1) * width + x + 1] * _mask3x3[2][2] + 
                        pixels[(y + 1) * width + x - 1] * _mask3x3[0][2] +
                        pixels[(y - 1) * width + x + 1] * _mask3x3[2][0] + 
                        pixels[(y - 1) * width + x - 1] * _mask3x3[0][0];

                    image.SetPixelGrey(x, y, (int)MathF.Round(pxl / 9f));
                }
            }

            image.ApplyChanges();
            return image;
        }
    }
}
