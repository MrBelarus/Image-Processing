
using System.IO;

namespace ImageProcessing.Core {
    public class ImageDetectDataProvider {
        private static ImageDetectDataProvider instance;
        public static ImageDetectDataProvider Instance {
            get {
                if (instance == null) {
                    instance = new ImageDetectDataProvider();
                }
                return instance;
            }
        }

        private ImageDetectData[] imageDetectDatas;
        public ImageDetectData[] ImageDetectDatas => imageDetectDatas;

        public ImageData zondRed;
        public ImageData zondBlue;

        public const string ImgRepoFolder = "ImgRepo\\";
        public const string ImgRepoZongFolder = "ImgRepo\\Zonds\\";

        public void LoadData() {
            imageDetectDatas = XML_FileManager.DeserializeFromXML<ImageDetectData[]>(XML_FileManager.PATH_DETECT_DATA);
            zondRed = new ImageData(ImgRepoZongFolder + "ZondRed.bmp");
            zondBlue = new ImageData(ImgRepoZongFolder + "ZondBlue.bmp");
        }

        public void SaveData() {
            XML_FileManager.SerializeToXML(imageDetectDatas, XML_FileManager.PATH_DETECT_DATA);
        }

        public void AddData(ImageDetectData data) {
            ImageDetectData[] newArray;
            if (imageDetectDatas == null) {
                newArray = new ImageDetectData[1];
            }
            else {
                newArray = new ImageDetectData[imageDetectDatas.Length + 1];
                for (int i = 0; i < imageDetectDatas.Length; i++) {

                    // override
                    if (imageDetectDatas[i].imgName == data.imgName) {
                        imageDetectDatas[i] = data;
                        return;
                    }

                    newArray[i] = imageDetectDatas[i];
                }
            }

            newArray[newArray.Length - 1] = data;
            imageDetectDatas = newArray;
        }

        public ImageDetectData Find(string imgName) {
            foreach(var data in imageDetectDatas) {
                if (data.className == imgName) {
                    return data;
                }
            }
            return null;
        }

        /// <returns>true if file was copied</returns>
        public bool CopyImgToBin(string pathFrom, string fileSafeName) {
            if (!Directory.Exists(ImgRepoFolder)) {
                Directory.CreateDirectory(ImgRepoFolder);
            }

            if (File.Exists(pathFrom)) {
                if (File.Exists(ImgRepoFolder + fileSafeName)) {
                    return false;
                }
                File.Copy(pathFrom, ImgRepoFolder + fileSafeName);
            }
            return true;
        }

        public int GetArrayIndexOf(ImageDetectData imageDetectData) {
            for (int i = 0; i < imageDetectDatas.Length; i++) {
                if (imageDetectData == imageDetectDatas[i]) {
                    return i;
                }
            }
            return -1;
        }
    }
}
