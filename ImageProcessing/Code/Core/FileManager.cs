using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImageProcessing.Core {
    class FileManager {
        public static void SaveImageMatrixTxt(string path, int[] array, int width, int height) {
            StringBuilder builder = new StringBuilder();
            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {
                    builder.Append(array[y * width + x]);
                    builder.Append(" ");
                }
                builder.Append("\n");
            }
            File.WriteAllText(path, builder.ToString());
        }
    }
}
