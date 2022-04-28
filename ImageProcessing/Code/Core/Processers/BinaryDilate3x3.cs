using ImageProcessing.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Core.Processers {
    public class BinaryDilate3x3 : ImageProcesser {
        int[][] _mask3x3 = new int[3][]
            {
                new int[3] {1, 1, 1},
                new int[3] {1, 1, 1},
                new int[3] {1, 1, 1},
            };

        public BinaryDilate3x3(int[][] mask3x3 = null) {
            if (mask3x3 != null) {
                _mask3x3 = mask3x3;
            }
        }

        public override ImageData Process(ImageData image) {
            if (image.ColorDepth != 1) {
                image = ImageUtility.ConvertToBinary(image, true);
            }

            int width = image.Width, height = image.Height;
            int[] pixels = image.GetPixels();

            //first and last strings
            int lowestStringStartInd = width * (height - 1);
            for (int x = 1; x < width - 1; x++) {

                for (int i = 0; i < 3; i++) {
                    for(int j = 1; j < 3; j++) {
                        if (i != j && _mask3x3[i][j] != 0) {
                            if (pixels[x + i - 1 + (j - 1) * width] == _mask3x3[i][j]) {
                                image.SetPixel(x, 0, 1);
                                break;
                            }
                        }
                    }
                }
                
                for (int i = 0; i < 3; i++) {
                    for (int j = 1; j < 3; j++) {
                        if (i != j && _mask3x3[i][j] != 0) {
                            if (pixels[lowestStringStartInd + x + i - 1 + (1 - j) * width] == _mask3x3[i][j]) {
                                image.SetPixel(x, height - 1, 1);
                                break;
                            }
                        }
                    }
                }
            }

            //first and last columns
            for (int y = 1; y < height - 1; y++) {

                for (int i = 1; i < 3; i++) {
                    for (int j = 0; j < 3; j++) {
                        if (i != j && _mask3x3[i][j] != 0) {
                            if (pixels[width * (y + j - 1) + i - 1] == _mask3x3[i][j]) {
                                image.SetPixel(0, y, 1);
                                break;
                            }
                        }
                    }
                }

                for (int i = 0; i < 2; i++) {
                    for (int j = 0; j < 3; j++) {
                        if (i != j && _mask3x3[i][j] != 0) {
                            if (pixels[width * (y + j) + i - 2] == _mask3x3[i][j]) {
                                image.SetPixel(width - 1, y, 1);
                                break;
                            }
                        }
                    }
                }
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
