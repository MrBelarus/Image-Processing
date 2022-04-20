using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Core.Processers {
    public class ConvertRGB_ToGreyMax : ImageProcesser {
        public override ImageData Process(ImageData image) {
            int pxl;
            for(int y = 0; y < image.Height; y++) {
                for(int x = 0; x < image.Width; x++) {
                    pxl = image.GetPixel(x, y);
                    int R = (pxl & 0x00ff0000) >> 16;
                    int G = (pxl & 0x0000ff00) >> 8;
                    int B = pxl & 0x000000ff;
                    //find max
                    if (R > B && R > G) {
                        pxl = R + (R << 8) + (R << 16);
                    }
                    else if(B > G) {
                        pxl = B + (B << 8) + (B << 16);
                    }
                    else {
                        pxl = G + (G << 8) + (G << 16);
                    }
                    //set
                    image.SetPixel(x, y, pxl);
                }
            }

            image.ApplyChanges();
            return image;
        }
    }
}
