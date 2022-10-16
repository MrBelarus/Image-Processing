
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

        public void LoadData() {
            imageDetectDatas = XML_FileManager.DeserializeFromXML<ImageDetectData[]>(XML_FileManager.PATH_DETECT_DATA);
        }
    }
}
