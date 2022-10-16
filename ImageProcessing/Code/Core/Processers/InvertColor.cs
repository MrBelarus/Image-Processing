using ImageProcessing.Utils;


namespace ImageProcessing.Core.Processers {
    public class InvertColor : ImageProcesser {
        public override ImageData Process(ImageData image) {
            int maxPixelValue = ImageUtility.GetMaxPixelValue(image.ColorDepth);

            for (int y = 0; y < image.Height; y++) {
                for (int x = 0; x < image.Width; x++) {
                    int pixel = image.GetPixel(x, y);
                    image.SetPixel(x, y, maxPixelValue - pixel);
                }
            }
            image.ApplyChanges();

            return image;
        }
    }
}
