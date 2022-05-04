using ImageProcessing.Utils;

namespace ImageProcessing.Core.Processers {
    public class BinaryErosion3x3 : ImageProcesser {
        int[][] _mask3x3 = new int[3][]
            {
                new int[3] {1, 1, 1},
                new int[3] {1, 1, 1},
                new int[3] {1, 1, 1},
            };

        public BinaryErosion3x3(int[][] mask3x3 = null) {
            if (mask3x3 != null) {
                _mask3x3 = mask3x3;
            }
            else {
                Matrix3x3Input matrix3x3Input = new Matrix3x3Input();
                matrix3x3Input.ShowDialog();

                if (matrix3x3Input.doApply != false) {
                    _mask3x3 = matrix3x3Input.Matrix3x3Int;
                }
            }
        }

        public override ImageData Process(ImageData image) {
            if (image.ColorDepth != 1) {
                image = ImageUtility.ConvertToBinary(image, true);
            }

            int width = image.Width, height = image.Height;
            int[] pixels = image.GetPixels();
            bool pixelWasChanged = false;

            //first and last strings
            int lowestStringStartInd = width * (height - 1);
            for (int x = 1; x < width - 1; x++) {

                pixelWasChanged = false;
                for (int i = 0; i < 3; i++) {
                    for (int j = 1; j < 3; j++) {
                        if (!(j == 1 && i == 1) && _mask3x3[j][i] != 0) {
                            if (pixels[x + i - 1 + (j - 1) * width] == 1) {
                                image.SetPixel(x, 0, 1);
                                pixelWasChanged = true;
                                break;
                            }
                        }
                    }
                    if (pixelWasChanged)
                        break;
                }
                if (!pixelWasChanged && IsMaskArrayNotZero(0, true)) {
                    image.SetPixel(x, 0, 1);
                }

                pixelWasChanged = false;
                for (int i = 0; i < 3; i++) {
                    for (int j = 1; j < 3; j++) {
                        if (!(j == 1 && i == 1) && _mask3x3[j][i] != 0) {
                            if (pixels[lowestStringStartInd + x + i - 1 + (1 - j) * width] == 1) {
                                image.SetPixel(x, height - 1, 1);
                                pixelWasChanged = true;
                                break;
                            }
                        }
                    }
                    if (pixelWasChanged)
                        break;
                }
                if (!pixelWasChanged && IsMaskArrayNotZero(2, true)) {
                    image.SetPixel(x, height - 1, 1);
                }
            }

            //first and last columns
            for (int y = 1; y < height - 1; y++) {

                pixelWasChanged = false;
                for (int i = 1; i < 3; i++) {
                    for (int j = 0; j < 3; j++) {
                        if (!(j == 1 && i == 1) && _mask3x3[j][i] != 0) {
                            if (pixels[width * (y + j - 1) + i - 1] == 1) {
                                image.SetPixel(0, y, 1);
                                pixelWasChanged = true;
                                break;
                            }
                        }
                    }
                    if (pixelWasChanged)
                        break;
                }
                if (!pixelWasChanged && IsMaskArrayNotZero(0, false)) {
                    image.SetPixel(0, y, 1);
                }

                pixelWasChanged = false;
                for (int i = 0; i < 2; i++) {
                    for (int j = 0; j < 3; j++) {
                        if (!(j == 1 && i == 1) && _mask3x3[j][i] != 0) {
                            if (pixels[width * (y + j) + i - 2] == 1) {
                                image.SetPixel(width - 1, y, 1);
                                pixelWasChanged = true;
                                break;
                            }
                        }
                    }
                    if (pixelWasChanged)
                        break;
                }
                if (!pixelWasChanged && IsMaskArrayNotZero(2, false)) {
                    image.SetPixel(width - 1, y, 1);
                }
            }


            //corners
            pixelWasChanged = false;
            for (int i = 1; i < 3; i++) {
                for (int j = 1; j < 3; j++) {
                    if (_mask3x3[i][j] != 0 && pixels[width * (j - 1) + i - 1] == 1) {
                        image.SetPixel(0, 0, 1);
                        pixelWasChanged = true;
                        break;
                    }
                }
                if (pixelWasChanged)
                    break;
            }
            if (!pixelWasChanged && (IsMaskArrayNotZero(arrayIndex: 0, checkHorizontaly: true) 
                                  || IsMaskArrayNotZero(arrayIndex: 0, checkHorizontaly: false))) {
                image.SetPixel(width - 1, 0, 1);
            }

            pixelWasChanged = false;
            for (int i = 0; i < 2; i++) {
                for (int j = 1; j < 3; j++) {
                    if (_mask3x3[i][j] != 0 && pixels[width * j + i - 2] == 1) {
                        image.SetPixel(width - 1, 0, 1);
                        pixelWasChanged = true;
                        break;
                    }
                }
                if (pixelWasChanged)
                    break;
            }
            if (!pixelWasChanged && (IsMaskArrayNotZero(arrayIndex: 0, checkHorizontaly: true)
                                  || IsMaskArrayNotZero(arrayIndex: 2, checkHorizontaly: false))) {
                image.SetPixel(0, height - 1, 1);
            }

            pixelWasChanged = false;
            for (int i = 1; i < 3; i++) {
                for (int j = 0; j < 2; j++) {
                    if (_mask3x3[i][j] != 0 && pixels[width * (height - 2 + j) + i - 1] == 1) {
                        image.SetPixel(0, height - 1, 1);
                        pixelWasChanged = true;
                        break;
                    }
                }
                if (pixelWasChanged)
                    break;
            }
            if (!pixelWasChanged && (IsMaskArrayNotZero(arrayIndex: 2, checkHorizontaly: true)
                                  || IsMaskArrayNotZero(arrayIndex: 0, checkHorizontaly: false))) {
                image.SetPixel(width - 1, height - 1, 1);
            }

            pixelWasChanged = false;
            for (int i = 0; i < 2; i++) {
                for (int j = 0; j < 2; j++) {
                    if (_mask3x3[i][j] != 0 && pixels[width * (height - 1 + j) + i - 2] == 1) {
                        image.SetPixel(width - 1, height - 1, 1);
                        pixelWasChanged = true;
                        break;
                    }
                }
                if (pixelWasChanged)
                    break;
            }
            if (!pixelWasChanged && (IsMaskArrayNotZero(arrayIndex: 2, checkHorizontaly: true)
                                  || IsMaskArrayNotZero(arrayIndex: 2, checkHorizontaly: false))) {
                image.SetPixel(0, 0, 1);
            }

            //main body
            for (int y = 1; y < height - 1; y++) {
                for (int x = 1; x < width - 1; x++) {
                    pixelWasChanged = false;
                    for (int i = 0; i < 3; i++) {
                        for (int j = 0; j < 3; j++) {
                            if (!(j == 1 && i == 1) && _mask3x3[j][i] != 0) {
                                if (pixels[width * (y + j - 1) + x + i - 1] == 1) {
                                    image.SetPixel(x, y, 1);
                                    pixelWasChanged = true;
                                    break;
                                }
                            }
                        }
                        if (pixelWasChanged)
                            break;
                    }
                }
            }

            image.ApplyChanges();
            return image;
        }

        private bool IsMaskArrayNotZero(int arrayIndex, bool checkHorizontaly) {
            if (checkHorizontaly) {
                for (int i = 0; i < 3; i++) {
                    if (_mask3x3[arrayIndex][i] != 0) {
                        return true;
                    }
                }
            }
            else {
                for (int i = 0; i < 3; i++) {
                    if (_mask3x3[i][arrayIndex] != 0) {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
