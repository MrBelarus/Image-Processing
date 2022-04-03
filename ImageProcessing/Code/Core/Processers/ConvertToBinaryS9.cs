
namespace ImageProcessing.Core.Processers {
    public class ConvertToBinaryS9 : ImageProcesser {
        public override ImageData Process(ImageData image) {
            int imgClrDepth = image.ColorDepth;
            if (imgClrDepth == 32) {
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
                        ((a8_matrix[i] + pixel) / 9) < pixel ? 0 : 0x00ffffff);
                }
            }
            image.ApplyChanges();

            return image;
        }
    }
}
