
namespace ImageProcessing.Code.Utils {
    class MathModule {
        public static float Lerp(float start, float end, float i) {
            return start + (end - start) * i;
        }

        public static float Clamp01(float value) {
            if (value < 0f) { return 0f; }
            if (value > 1f) { return 1f; }
            return value;
        }

        public static float Clamp(float value, float min, float max) {
            if (value < min) { return min; }
            if (value > max) { return max; }
            return value;
        }

        public static int Clamp(int value, int min, int max) {
            if (value < min) { return min; }
            if (value > max) { return max; }
            return value;
        }
    }
}
