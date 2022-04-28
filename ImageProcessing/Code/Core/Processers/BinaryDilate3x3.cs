using ImageProcessing.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Core.Processers {
    public class BinaryDilate3x3 : ImageProcesser {
        int[][] _mask3x3 = new int[3][]
            {
                new int[3] {1, 1, 1},
                new int[3] {1, 1, 1},
                new int[3] {1, 1, 1},
            };

        public BinaryDilate3x3(int[][] mask3x3 = null) {
            if (mask3x3 != null) {
                _mask3x3 = mask3x3;
            }
        }

        public override ImageData Process(ImageData image) {
            if (image.ColorDepth != 1) {
                image = ImageUtility.ConvertToBinary(image, true);
            }

            int width = image.Width, height = image.Height;
            int[] pixels = image.GetPixels();

            //first and last strings
            int lowestStringStartInd = width * (height - 1);
            for (int x = 1; x < width - 1; x++) {

                for (int i = 0; i < 3; i++) {
                    for (int j = 1; j < 3; j++) {
                        if (i != 1 && j != 1 || _mask3x3[i][j] != 0) {
                            if (pixels[x + i - 1 + (j - 1) * width] == _mask3x3[i][j] - 1) {
                                image.SetPixel(x, 0, 0);
                                break;
                            }
                        }
                    }
                }

                for (int i = 0; i < 3; i++) {
                    for (int j = 1; j < 3; j++) {
                        if (i != 1 && j != 1 || _mask3x3[i][j] != 0) {
                            if (pixels[lowestStringStartInd + x + i - 1 + (1 - j) * width] == _mask3x3[i][j] - 1) {
                                image.SetPixel(x, height - 1, 0);
                                break;
                            }
                        }
                    }
                }
            }

            //first and last columns
            for (int y = 1; y < height - 1; y++) {

                for (int i = 1; i < 3; i++) {
                    for (int j = 0; j < 3; j++) {
                        if (i != 1 && j != 1 || _mask3x3[i][j] != 0) {
                            if (pixels[width * (y + j - 1) + i - 1] == _mask3x3[i][j] - 1) {
                                image.SetPixel(0, y, 0);
                                break;
                            }
                        }
                    }
                }

                for (int i = 0; i < 2; i++) {
                    for (int j = 0; j < 3; j++) {
                        if (i != 1 && j != 1 || _mask3x3[i][j] != 0) {
                            if (pixels[width * (y + j) + i - 2] == _mask3x3[i][j] - 1) {
                                image.SetPixel(width - 1, y, 0);
                                break;
                            }
                        }
                    }
                }
            }


            //corners
            for (int i = 1; i < 3; i++) {
                for (int j = 1; j < 3; j++) {
                    if (pixels[width * (j - 1) + i - 1] == _mask3x3[i][j] - 1) {
                        image.SetPixel(0, 0, 0);
                        break;
                    }
                }
            }
            for (int i = 0; i < 2; i++) {
                for (int j = 1; j < 3; j++) {
                    if (pixels[width * j + i - 2] == _mask3x3[i][j] - 1) {
                        image.SetPixel(width - 1, 0, 0);
                        break;
                    }
                }
            }
            for (int i = 1; i < 3; i++) {
                for (int j = 0; j < 2; j++) {
                    if (pixels[width * (height - 2 + j) + i - 1] == _mask3x3[i][j] - 1) {
                        image.SetPixel(width - 1, 0, 0);
                        break;
                    }
                }
            }
            for (int i = 0; i < 2; i++) {
                for (int j = 0; j < 2; j++) {
                    if (pixels[width * (height - 1 + j) + i - 2] == _mask3x3[i][j] - 1) {
                        image.SetPixel(width - 1, 0, 0);
                        break;
                    }
                }
            }

            //main body
            for (int y = 1; y < height - 1; y++) {
                for (int x = 1; x < width - 1; x++) {

                    for (int i = 0; i < 3; i++) {
                        for (int j = 0; j < 3; j++) {
                            if (i != j && i != 1 || _mask3x3[i][j] != 0) {
                                if (pixels[width * (y + j - 1) + x + i - 1] == _mask3x3[i][j] - 1) {
                                    image.SetPixel(x, y, 0);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            image.ApplyChanges();
            return image;
        }
    }
}
