using ImageProcessing.Core;
using ImageProcessing.Utils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ImageProcessing.Code.Core {
    class ImgToGrey : ImageProcesser {

        // Access pixel at (x,y) [RGBA32]
        //B = bytes[bitmapData.Scan0 + x * 4 + y * bitmapData.Stride + 0]
        //G = bytes[bitmapData.Scan0 + x * 4 + y * bitmapData.Stride + 1]
        //R = bytes[bitmapData.Scan0 + x * 4 + y * bitmapData.Stride + 2]
        //A = bytes[bitmapData.Scan0 + x * 4 + y * bitmapData.Stride + 3]

        // Access pixel at (x,y) [RGBA24]
        //B = bytes[bitmapData.Scan0 + x * 4 + y * bitmapData.Stride + 0]
        //G = bytes[bitmapData.Scan0 + x * 4 + y * bitmapData.Stride + 1]
        //R = bytes[bitmapData.Scan0 + x * 4 + y * bitmapData.Stride + 2]

        //https://stackoverflow.com/questions/51071944/how-can-i-work-with-1-bit-and-4-bit-images

        public override Bitmap Process(Bitmap bitmap) {
            int colorDepth = ImageManager.GetColorDepth(bitmap);
            Tuple<byte[], int> imgBytesData = ImageManager.BitmapToBytes(bitmap, bitmap.PixelFormat);

            //string data = "";
            //for(int i = 0; i < imgBytesData.Item1.Length; i++) {
            //    data += ((imgBytesData.Item1[i] & 0b10000000) >> 7).ToString();
            //    data += ((imgBytesData.Item1[i] & 0b01000000)>> 6).ToString();
            //    data += ((imgBytesData.Item1[i] & 0b00100000) >> 5).ToString();
            //    data += ((imgBytesData.Item1[i] & 0b00010000) >> 4).ToString();
            //    data += ((imgBytesData.Item1[i] & 0b00001000) >> 3).ToString();
            //    data += ((imgBytesData.Item1[i] & 0b00000100) >> 2).ToString();
            //    data += ((imgBytesData.Item1[i] & 0b00000010) >> 1).ToString();
            //    data += (imgBytesData.Item1[i] & 0b00000001).ToString();
            //    if (i != 0 && i % imgBytesData.Item2 == 0) {
            //        data += "\n";
            //    }
            //}
            //File.WriteAllText("C:\\Users\\vladk\\source\\repos\\ImageProcessing\\ImageProcessing\\bin\\Debug\\netcoreapp3.1\\imgBytes", data);

            return ImageManager.BitmapFromBytes(imgBytesData.Item1, bitmap.Width, bitmap.Height, bitmap.PixelFormat);
        }
    }
}
