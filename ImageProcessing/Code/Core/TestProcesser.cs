using ImageProcessing.Core;
using System;

namespace ImageProcessing.Code.Core {
    class TestProcesser : ImageProcesser {
        public override ImageData Process(ImageData image) {
            switch (image.ColorDepth) {
                case 1:
                    _1bitImage(image);
                    break;
                case 4:
                    _4bitImage(image);
                    break;
                case 8:
                    _8bitImage(image);
                    break;
                case 16:
                    //
                    break;
                case 24:
                    //
                    break;
                case 32:
                    //
                    break;
                default:
                    throw new Exception("Not supported image format");
            }
            return image;
        }

        private void _1bitImage(ImageData image) {
            for (int y = 0; y < image.Height; y++) {
                for (int x = 0; x < image.Width; x++) {
                    image.SetPixel(x, y, 5);
                }
            }
            image.ApplyChanges();
        }

        private void _4bitImage(ImageData image) {
            for(int y = 0; y < image.Height; y++) {
                for(int x = 0; x < image.Width; x++) {
                    image.SetPixel(x, y, 5);
                }
            }
            image.ApplyChanges();
        }

        private void _8bitImage(ImageData image) {
            for (int y = 0; y < image.Height; y++) {
                for (int x = 0; x < image.Width; x++) {
                    image.SetPixel(x, y, 5);
                }
            }
            image.ApplyChanges();
        }
    }
}
