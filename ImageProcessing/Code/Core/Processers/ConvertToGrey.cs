using ImageProcessing.Utils;


namespace ImageProcessing.Core.Processers {
    public class ConvertToGrey : ImageProcesser {
        public override ImageData Process(ImageData image) {
            if (image.ColorDepth != 1) {
                image = ImageUtility.ConvertToBinary(image, true, 128);
            }
            return ImageUtility.Convert1BitToGray24Bit(image);
        }
    }
}
