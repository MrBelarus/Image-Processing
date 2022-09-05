using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Core {
    class CoOccurenceMatrixCalculator {
        const int dimension = 256;
        public static int[] GetImageMatrix_C90_D1(ImageData imageData) {
            int width = imageData.Width, height = imageData.Height;

            Processers.ConvertRGB_ToGreyMax converter = new Processers.ConvertRGB_ToGreyMax();
            imageData = converter.Process(imageData);

            int[] pixels = imageData.GetPixels();

            int[] result = new int[dimension * dimension];

            for (int x = 0; x < width; x++) {
                for (int y = 1; y < height; y++) {
                    int pxl1 = pixels[y * width + x];
                    int pxl2 = pixels[(y - 1) * width + x];

                    result[pxl1 * dimension + pxl2]++;
                }
            }

            return result;
        }

        public static float[] GetNormalizedMatrix(int[] matrix) {
            float[] result = new float[matrix.Length];

            int sum = 0;
            foreach(var val in matrix) {
                sum += val;
            }

            for(int i = 0; i < matrix.Length; i++) {
                result[i] = ((float)matrix[i]) / sum;
            }

            return result;
        }

    }
}
