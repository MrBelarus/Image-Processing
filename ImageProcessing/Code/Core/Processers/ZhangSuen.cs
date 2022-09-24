using ImageProcessing.Utils;
using System;

namespace ImageProcessing.Core.Processers {
    public class ZhangSuen : ImageProcesser {
        private int[] pixels;
        private int width;
        private int height;
        private bool wasAnyPixelDeleted;

        private static bool isFirstIteration = true;

        public override ImageData Process(ImageData image) {
            width = image.Width;
            height = image.Height;
            if (image.ColorDepth != 1) {
                image = ImageUtility.ConvertToBinary(image, true, 128);
            }
            pixels = image.GetPixels();

            wasAnyPixelDeleted = true;
            bool firstIteration = true;
            bool otherIterationWas = true;
            while (wasAnyPixelDeleted && otherIterationWas) {
                otherIterationWas = wasAnyPixelDeleted;
                DeletePixels(firstIteration);
                firstIteration = !firstIteration;
            }

            //DeletePixels(isFirstIteration);
            //isFirstIteration = !isFirstIteration;

            for (int y = 0; y < image.Height; y++) {
                for (int x = 0; x < image.Width; x++) {
                    image.SetPixel(x, y, pixels[y * width + x]);
                }
            }

            image.ApplyChanges();
            return image;
        }

        private int GetTransitionsCount(params int[] values) {
            int sum = 0;
            for (int i = 1; i < values.Length; i++) {
                if (values[i - 1] - values[i] == 1) {
                    sum++;
                }
            }
            return sum;
        }

        private void DeletePixels(bool isFirstIteration) {
            int p2, p3, p4, p5, p6, p7, p8, p9;
            wasAnyPixelDeleted = false;

            int[] pixelsNew = new int[pixels.Length];
            Array.Copy(pixels, pixelsNew, pixels.Length);

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (pixels[y * width + x] == 1) {
                        continue;
                    }

                    p2 = y == 0 ? 1 : pixels[(y - 1) * width + x];
                    p3 = (y == 0 || x == width - 1) ? 1 : pixels[(y - 1) * width + x + 1];
                    p4 = (x == width - 1) ? 1 : pixels[y * width + x + 1];
                    p5 = (y == height - 1 || x == width - 1) ? 1 : pixels[(y + 1) * width + x + 1];
                    p6 = (y == height - 1) ? 1 : pixels[(y + 1) * width + x];
                    p7 = (y == height - 1 || x == 0) ? 1 : pixels[(y + 1) * width + x - 1];
                    p8 = x == 0 ? 1 : pixels[y * width + x - 1];
                    p9 = (y == 0 || x == 0) ? 1 : pixels[(y - 1) * width + x - 1];

                    int a8 = (p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9);
                    bool B = a8 >= 2 && a8 <= 6;
                    bool A = GetTransitionsCount(p2, p3, p4, p5, p6, p7, p8, p9, p2) == 1;

                    bool firstCondition = isFirstIteration ? (p2 == 1 || p4 == 1 || p6 == 1)
                                                           : (p2 == 1 || p4 == 1 || p8 == 1) ;
                    bool secondCondition = isFirstIteration ? (p4 == 1 || p6 == 1 || p8 == 1)
                                                            : (p2 == 1 || p6 == 1 || p8 == 1);

                    if (A && B && firstCondition && secondCondition) {
                        wasAnyPixelDeleted = true;
                        pixelsNew[y * width + x] = 1;
                    }
                }
            }

            if (wasAnyPixelDeleted) {
                pixels = pixelsNew;
            }
        }
    }
}
