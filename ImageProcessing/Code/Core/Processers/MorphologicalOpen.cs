﻿
namespace ImageProcessing.Core.Processers {
    public class MorphologicalOpen : ImageProcesser {
        int[][] _mask3x3 = new int[3][]
        {
                new int[3] {1, 1, 1},
                new int[3] {1, 1, 1},
                new int[3] {1, 1, 1},
        };

        public MorphologicalOpen(int[][] mask3x3 = null) {
            if (mask3x3 != null) {
                _mask3x3 = mask3x3;
            }
        }

        public override ImageData Process(ImageData image) {
            image = new BinaryErosion3x3(_mask3x3).Process(image);
            image = new BinaryDilate3x3(_mask3x3).Process(image);
            return image;
        }
    }
}
