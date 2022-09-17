
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
                    int pxl1 = pixels[y * width + x] & 0xFF; // & 0xFF will apply one channel mask (we will get value 0-255)
                    int pxl2 = pixels[(y - 1) * width + x] & 0xFF;

                    result[(pxl1 * dimension) + pxl2]++;
                }
            }

            return result;
        }

        public static float GetEnergy(float[] matrix) {
            float result = 0;
            for (int i = 0; i < matrix.Length; i++) {
                result += matrix[i] * matrix[i];
            }
            return result;
        }

        public static float GetEntrophy(float[] matrix) {
            double result = 0;
            for (int i = 0; i < matrix.Length; i++) {
                float value = matrix[i];
                if (value > 0) {
                    result += value * System.Math.Log2(value);
                }
            }
            return (float)(result != 0 ? -result : result);
        }

        public static float GetContrast(float[] matrix) {
            float result = 0;
            for (int i = 0; i < matrix.Length; i++) {
                int x = i / dimension;
                int y = i % dimension;
                result += (x - y) * (x - y) * matrix[i];
            }
            return result;
        }

        public static float GetHomogen(float[] matrix) {
            float result = 0;
            for (int i = 0; i < matrix.Length; i++) {
                int x = i / dimension;
                int y = i % dimension;
                result += matrix[i] / (1 + System.Math.Abs(x - y));
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
