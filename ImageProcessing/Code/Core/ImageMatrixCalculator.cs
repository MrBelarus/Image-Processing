﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ImageProcessing.Core {
    class ImageMatrixCalculator {
        private static int[] pixels;
        private static int imgWidth;
        private static int imgHeight;

        /// <summary>
        /// returns matrix where white is 0 and black is 1
        /// </summary>
        public static int[] GetA8Matrix(ImageData imageData, bool onlyForBlackPixels = false) {
            int width = imageData.Width, height = imageData.Height;
            int[] a8matrix = new int[width * height];

            int[] pixels = imageData.GetPixels();

            int p2, p3, p4, p5, p6, p7, p8, p9;

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (onlyForBlackPixels && pixels[y * width + x] != 0) {
                        continue;
                    }

                    p2 = y == 0 ? 1 : pixels[(y - 1) * width + x];
                    p3 = (y == 0 || x == width - 1) ? 1 : pixels[(y - 1) * width + x + 1];
                    p4 = (x == width - 1) ? 1 : pixels[y * width + x + 1];
                    p5 = (y == height - 1 || x == width - 1) ? 1 : pixels[(y + 1) * width + x + 1];
                    p6 = (y == height - 1) ? 1 : pixels[(y + 1) * width + x];
                    p7 = (y == height - 1 || x == 0) ? 1 : pixels[(y + 1) * width + x - 1];
                    p8 = x == 0 ? 1 : pixels[y * width + x - 1];
                    p9 = (y == 0 || x == 0) ? 1 : pixels[(y - 1) * width + x - 1];


                    a8matrix[y * width + x] =
                            8 - (p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9);
                }
            }

            return a8matrix;
        }

        /// <summary>
        /// returns matrix where white is 1 and black is 0
        /// </summary>
        public static int[] GetA8(ImageData imageData, bool onlyForBlackPixels = false) {
            int width = imageData.Width, height = imageData.Height;
            int[] a8matrix = new int[width * height];

            int[] pixels = imageData.GetPixels();

            int p2, p3, p4, p5, p6, p7, p8, p9;

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (onlyForBlackPixels && pixels[y * width + x] != 0) {
                        continue;
                    }

                    p2 = y == 0 ? 1 : pixels[(y - 1) * width + x];
                    p3 = (y == 0 || x == width - 1) ? 1 : pixels[(y - 1) * width + x + 1];
                    p4 = (x == width - 1) ? 1 : pixels[y * width + x + 1];
                    p5 = (y == height - 1 || x == width - 1) ? 1 : pixels[(y + 1) * width + x + 1];
                    p6 = (y == height - 1) ? 1 : pixels[(y + 1) * width + x];
                    p7 = (y == height - 1 || x == 0) ? 1 : pixels[(y + 1) * width + x - 1];
                    p8 = x == 0 ? 1 : pixels[y * width + x - 1];
                    p9 = (y == 0 || x == 0) ? 1 : pixels[(y - 1) * width + x - 1];


                    a8matrix[y * width + x] =
                            p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9;
                }
            }

            return a8matrix;
        }

        public static int[] GetB8Matrix(ImageData imageData, bool onlyForBlackPixels = false) {
            int width = imageData.Width, height = imageData.Height;
            int[] b8matrix = new int[width * height];

            int[] pixels = imageData.GetPixels();

            int p2, p3, p4, p5, p6, p7, p8, p9;

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (onlyForBlackPixels && pixels[y * width + x] != 0) {
                        continue;
                    }
                    p2 = y == 0 ? 1 : pixels[(y - 1) * width + x];
                    p3 = (y == 0 || x == width - 1) ? 1 : pixels[(y - 1) * width + x + 1];
                    p4 = (x == width - 1) ? 1 : pixels[y * width + x + 1];
                    p5 = (y == height - 1 || x == width - 1) ? 1 : pixels[(y + 1) * width + x + 1];
                    p6 = (y == height - 1) ? 1 : pixels[(y + 1) * width + x];
                    p7 = (y == height - 1 || x == 0) ? 1 : pixels[(y + 1) * width + x - 1];
                    p8 = x == 0 ? 1 : pixels[y * width + x - 1];
                    p9 = (y == 0 || x == 0) ? 1 : pixels[(y - 1) * width + x - 1];

                    b8matrix[y * width + x] =
                            GetPairsCount(p2, p3, p4, p5, p6, p7, p8, p9, p2);
                }
            }

            return b8matrix;
        }

        public static int[] GetCnMatrix(ImageData imageData, bool onlyForBlackPixels = true) {
            int[] a8 = GetA8Matrix(imageData, onlyForBlackPixels);
            int[] b8 = GetB8Matrix(imageData, onlyForBlackPixels);
            var result = new int[a8.Length];
            var pixels = imageData.GetPixels();

            for (int i = 0; i < a8.Length; i++) {
                if (onlyForBlackPixels && pixels[i] != 0) {
                    continue;
                }
                result[i] = a8[i] - b8[i];
            }
            return result;
        }

        private static int GetPairsCount(params int[] pixels) {
            int count = 0;
            for (int i = 0; i < pixels.Length - 1; i++) {
                if (pixels[i] == 0 && pixels[i + 1] == 0) {
                    count++;
                }
            }
            return count;
        }

        public static int[] GetA4Matrix(ImageData imageData) {
            int width = imageData.Width, height = imageData.Height;
            int[] a4matrix = new int[width * height];

            int[] pixels = imageData.GetPixels();

            //first and last strings
            int lowestStringStartInd = width * (height - 1);
            for (int x = 1; x < width - 1; x++) {
                a4matrix[x] = pixels[x - 1] + pixels[x + 1] + pixels[width + x];

                a4matrix[lowestStringStartInd + x] = pixels[lowestStringStartInd + x - 1] +
                    pixels[lowestStringStartInd + x + 1] + pixels[lowestStringStartInd + x - width];
            }

            //first and last columns
            for (int y = 1; y < height - 1; y++) {
                a4matrix[width * y] = pixels[width * y + 1] +
                    pixels[width * (y - 1)] + pixels[width * (y + 1)];

                a4matrix[width * (y + 1) - 1] = pixels[width * (y + 1) - 2] +
                    pixels[width * y - 1] + pixels[width * (y + 2) - 1];
            }

            //main body
            for (int y = 1; y < imageData.Height - 1; y++) {
                for (int x = 1; x < width - 1; x++) {
                    a4matrix[y * width + x] = pixels[(y - 1) * width + x] + pixels[(y + 1) * width + x] +
                        pixels[y * width + x - 1] + pixels[y * width + x + 1];
                }
            }

            return a4matrix;
        }


        #region Manhattan Distance
        public static int[] GetManhattanDistanceMatrix(ImageData imageData) {
            imgHeight = imageData.Height;
            imgWidth = imageData.Width;
            pixels = imageData.GetPixels();

            int[] distMatrix = new int[imgWidth * imgHeight];
            for (int y = 0; y < imgHeight; y++) {
                for (int x = 0; x < imgWidth; x++) {
                    distMatrix[x + y * imgWidth] = GetManhattanDistance(x, y);
                }
            }

            return distMatrix;
        }

        private static int curDist;
        private static int minDist;
        private static int lookingFor;
        private static int GetManhattanDistance(int x, int y) {
            lookingFor = pixels[x + y * imgWidth] == 1 ? 0 : 1;

            int maxRadius = x;
            if (y > maxRadius) {
                maxRadius = y;
            }
            if (imgWidth - x > maxRadius) {
                maxRadius = imgWidth - x;
            }
            if (imgHeight - y > maxRadius) {
                maxRadius = imgHeight - y;
            }

            int maxLeft, maxRight, maxUp, maxDown;
            minDist = int.MaxValue;
            for (int r = 1; r < maxRadius; r++) {
                maxLeft = Math.Max(-r, -x);
                maxRight = Math.Min(r, imgWidth - x - 1);
                maxDown = Math.Min(r, imgHeight - y - 1);
                maxUp = Math.Max(-r, -y);

                for (int h = maxUp; h <= maxDown; h++) {
                    CheckForMinDistance(x, y, maxLeft, h);
                    CheckForMinDistance(x, y, maxRight, h);
                    if (minDist == r) {
                        return lookingFor == 0 ? minDist : -minDist;
                    }
                }

                for (int w = maxLeft + 1; w < maxRight; w++) {
                    CheckForMinDistance(x, y, w, maxUp);
                    CheckForMinDistance(x, y, w, maxDown);
                    if (minDist == r) {
                        return lookingFor == 0 ? minDist : -minDist;
                    }
                }

                if (minDist != int.MaxValue) {
                    maxRadius = minDist;
                    //return lookingFor == 0 ? minDist : -minDist;
                }
            }

            return lookingFor == 0 ? minDist : -minDist;
        }

        private static void CheckForMinDistance(int x, int y, int offsetX, int offsetY) {
            int pxlValue = pixels[x + offsetX + (y + offsetY) * imgWidth];
            if (pxlValue != lookingFor) {
                return;
            }
            curDist = Math.Abs(offsetY) + Math.Abs(offsetX);
            if (curDist < minDist) {
                minDist = curDist;
            }
        }

        /// <summary>
        /// X = value, Y = frequency
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<OxyPlot.DataPoint> CalculateValuesFrequences(int[] values) {
            List<OxyPlot.DataPoint> points = new List<OxyPlot.DataPoint>();

            List<int> valuesList = values.ToList();

            while (valuesList.Count > 0) {
                int valueCount = 1;
                int value = valuesList[0];
                valuesList.RemoveAt(0);

                for (int j = 0; j < valuesList.Count; j++) {
                    if (valuesList[j] == value) {
                        valuesList.RemoveAt(j);
                        valueCount++;
                        j--;
                    }
                }

                //sorted inserting
                bool flag = true;
                for (int j = 0; j < points.Count; j++) {
                    if (points[j].X > value) {
                        points.Insert(j, new OxyPlot.DataPoint(value, valueCount));
                        flag = false;
                        break;
                    }
                }
                if (flag) {
                    points.Add(new OxyPlot.DataPoint(value, valueCount));
                }
            }

            return points;
        }

        #endregion


        public static int[] ConvertDistanceToPixelGreyValues(int[] distances) {
            int minDistance = distances.Min();
            int maxDistance = distances.Max();
            int amp = maxDistance - minDistance;

            int[] result = new int[distances.Length];

            for (int i = 0; i < distances.Length; i++) {
                result[i] = (distances[i] - minDistance) * 255 / amp;
            }

            return result;
        }
    }
}
