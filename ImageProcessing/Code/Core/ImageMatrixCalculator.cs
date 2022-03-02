using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Code.Core {
    class ImageMatrixCalculator {
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
    }
}
