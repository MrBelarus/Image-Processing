using ImageProcessing.Code.Core;
using System.Drawing;

namespace ImageProcessing.Core {
    public abstract class ImageProcesser {
        protected Bitmap _result;
        public Bitmap Result => _result;

        public abstract ImageData Process(ImageData bitmap);
    }
}
