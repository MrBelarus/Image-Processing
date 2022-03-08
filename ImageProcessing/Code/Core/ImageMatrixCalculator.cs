using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Code.Core {
    class ImageMatrixCalculator {
        private static int[] pixels;
        private static int imgWidth;
        private static int imgHeight;

        public static int[] GetA8Matrix(ImageData imageData) {
            int width = imageData.Width, height = imageData.Height;
            int[] a8matrix = new int[width * height];

            int[] pixels = imageData.GetPixels();

            //first and last strings
            int lowestStringStartInd = width * (height - 1);
            for (int x = 1; x < width - 1; x++) {
                a8matrix[x] = pixels[x - 1] + pixels[x + 1] + pixels[width + x]
                    + pixels[width + x - 1] + pixels[width + x + 1];

                a8matrix[lowestStringStartInd + x] = pixels[lowestStringStartInd + x - 1] +
                    pixels[lowestStringStartInd + x + 1] + pixels[lowestStringStartInd + x - width] +
                    pixels[lowestStringStartInd - width + x - 1] + pixels[lowestStringStartInd - width + x + 1];
            }

            //first and last columns
            for (int y = 1; y < height - 1; y++) {
                a8matrix[width * y] = pixels[width * y + 1] +
                    pixels[width * (y - 1)] + pixels[width * (y + 1)] +
                    pixels[width * (y - 1) + 1] + pixels[width * (y + 1) + 1];

                a8matrix[width * (y + 1) - 1] = pixels[width * (y + 1) - 2] +
                    pixels[width * y - 1] + pixels[width * (y + 2) - 1] +
                    pixels[width * (y + 2) - 2] + pixels[width * y - 2];
            }

            //main body
            for (int y = 1; y < imageData.Height - 1; y++) {
                for (int x = 1; x < width - 1; x++) {
                    a8matrix[y * width + x] = pixels[(y - 1) * width + x] + pixels[(y + 1) * width + x] +
                        pixels[y * width + x - 1] + pixels[y * width + x + 1] +
                        pixels[(y + 1) * width + x + 1] + pixels[(y + 1) * width + x - 1] +
                        pixels[(y - 1) * width + x + 1] + pixels[(y - 1) * width + x - 1];
                }
            }

            return a8matrix;
        }

        public static int[] GetA4Matrix(ImageData imageData) {
            int width = imageData.Width, height = imageData.Height;
            int[] a4matrix = new int[width * height];

            int[] pixels = imageData.GetPixels();

            //first and last strings
            int lowestStringStartInd = width * (height - 1);
            for (int x = 1; x < width - 1; x++) {
                a4matrix[x] = pixels[x - 1] + pixels[x + 1] + pixels[width + x];

                a4matrix[lowestStringStartInd + x] = pixels[lowestStringStartInd + x - 1] +
                    pixels[lowestStringStartInd + x + 1] + pixels[lowestStringStartInd + x - width];
            }

            //first and last columns
            for (int y = 1; y < height - 1; y++) {
                a4matrix[width * y] = pixels[width * y + 1] +
                    pixels[width * (y - 1)] + pixels[width * (y + 1)];

                a4matrix[width * (y + 1) - 1] = pixels[width * (y + 1) - 2] +
                    pixels[width * y - 1] + pixels[width * (y + 2) - 1];
            }

            //main body
            for (int y = 1; y < imageData.Height - 1; y++) {
                for (int x = 1; x < width - 1; x++) {
                    a4matrix[y * width + x] = pixels[(y - 1) * width + x] + pixels[(y + 1) * width + x] +
                        pixels[y * width + x - 1] + pixels[y * width + x + 1];
                }
            }

            return a4matrix;
        }

        public static int[] GetManhattanDistanceMatrix(ImageData imageData) {
            imgHeight = imageData.Height;
            imgWidth = imageData.Width;
            pixels = imageData.GetPixels();

            int[] distMatrix = new int[imgWidth * imgHeight];
            for(int y = 0; y < imgHeight; y++) {
                for(int x = 0; x < imgWidth; x++) {
                    distMatrix[x + y * imgWidth] = GetManhattanDistance(x, y);
                }
            }

            return distMatrix;
        }

        private static int curDist;
        private static int minDist;
        private static int lookingFor;
        private static int GetManhattanDistance(int x, int y) {
            lookingFor = pixels[x + y * imgWidth] == 1 ? 0 : 1;

            int maxRadius = x;
            if (y > maxRadius) {
                maxRadius = y;
            }
            if (imgWidth - x > maxRadius) {
                maxRadius = imgWidth - x;
            }
            if (imgHeight - y > maxRadius) {
                maxRadius = imgHeight - y;
            }

            int maxLeft, maxRight, maxUp, maxDown;
            minDist = int.MaxValue;
            for (int r = 1; r < maxRadius; r++) {
                maxLeft = Math.Max(-r, -x);
                maxRight = Math.Min(r, imgWidth - x - 1);
                maxDown = Math.Min(r, imgHeight - y - 1);
                maxUp = Math.Max(-r, -y);

                for (int h = maxUp; h <= maxDown; h++) {
                    CheckForMinDistance(x, y, maxLeft, h);
                    CheckForMinDistance(x, y, maxRight, h);
                    if (minDist == r) {
                        return lookingFor == 0 ? minDist : -minDist;
                    }
                }

                for (int w = maxLeft + 1; w < maxRight; w++) {
                    CheckForMinDistance(x, y, w, maxUp);
                    CheckForMinDistance(x, y, w, maxDown);
                    if (minDist == r) {
                        return lookingFor == 0 ? minDist : -minDist;
                    }
                }

                if (minDist != int.MaxValue) {
                    return lookingFor == 0 ? minDist : -minDist;
                }
            }

            return int.MinValue;
        }

        private static void CheckForMinDistance(int x, int y, int offsetX, int offsetY) {
            int pxlValue = pixels[x + (y + offsetY) * imgWidth + offsetX];
            if (pxlValue != lookingFor) {
                return;
            }
            curDist = Math.Abs(offsetY) + Math.Abs(offsetX);
            if (curDist < minDist) {
                minDist = curDist;
            }
        }
    }
}
