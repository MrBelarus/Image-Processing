using ImageProcessing.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Code.Core {
    class TestProcesser : ImageProcesser {
        public override ImageData Process(ImageData image) {
            switch (image.ColorDepth) {
                case 4:
                    _4bitImage(image);
                    break;
            }
            return image;
        }

        private void _4bitImage(ImageData image) {
            for(int y = 0; y < image.Height; y++) {
                for(int x = 0; x < image.Width; x++) {
                    image.SetPixel(x, y, 5);
                }
            }
            image.ApplyChanges();
        }
    }
}
