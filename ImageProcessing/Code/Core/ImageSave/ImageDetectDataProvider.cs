
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

        public const string ImgRepoFolder = "ImgRepo\\";

        public void LoadData() {
            imageDetectDatas = XML_FileManager.DeserializeFromXML<ImageDetectData[]>(XML_FileManager.PATH_DETECT_DATA);
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
                    newArray[i] = imageDetectDatas[i];
                }
            }
            
            newArray[newArray.Length - 1] = data;
            imageDetectDatas = newArray;
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
    }
}
