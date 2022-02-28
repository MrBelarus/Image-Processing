using System.Drawing;

namespace ImageProcessing.Core {
    public abstract class ImageProcesser {
        protected Bitmap _result;
        public Bitmap Result => _result;

        public abstract Bitmap Process(Bitmap bitmap);
    }
}
