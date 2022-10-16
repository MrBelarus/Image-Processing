using System.IO;
using System.Xml.Serialization;

namespace ImageProcessing.Core {
    public class XML_FileManager {
        public const string PATH_DETECT_DATA = "img_detect_db.xml";

        public static void SerializeToXML<T>(T data, string path) {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            FileStream fileStream;
            if (!File.Exists(path)) {
                fileStream = File.Create(path);
            }
            else {
                fileStream = new FileStream(path, FileMode.Open);
            }
            serializer.Serialize(fileStream, data);
            fileStream.Close();
        }

        public static T DeserializeFromXML<T>(string path) {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            FileStream fileStream = new FileStream(path, FileMode.Open);
            T result = (T)serializer.Deserialize(fileStream);
            fileStream.Close();
            return result;
        }
    }
}
