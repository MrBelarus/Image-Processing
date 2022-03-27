using System.Drawing;

namespace ImageProcessing.Core.Processers {
    /// <summary>
    /// Base class for all image processers
    /// </summary>
    public abstract class ImageProcesser {
        protected Bitmap _result;
        public Bitmap Result => _result;

        public abstract ImageData Process(ImageData image);
    }
}
