using AurecasLib.Surrogate;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
namespace AurecasLib.Saving {

    public class SaveSystem {
        public static bool logFilePath;
        public static void SaveGame<T>(T data) {
            BinaryFormatter bf = new BinaryFormatter();
            SurrogateSelector surrogateSelector = new SurrogateSelector();

            //Vector3 surrogate
            Vector3SerializationSurrogate vector3SS = new Vector3SerializationSurrogate();
            surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SS);

            //Vector2 surrogate
            Vector2SerializationSurrogate vector2SS = new Vector2SerializationSurrogate();
            surrogateSelector.AddSurrogate(typeof(Vector2), new StreamingContext(StreamingContextStates.All), vector2SS);

            //Quaternion surrogate
            QuaternionSerializationSurrogate quaternionSS = new QuaternionSerializationSurrogate();
            surrogateSelector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternionSS);

            bf.SurrogateSelector = surrogateSelector;

            if (Debug.isDebugBuild && logFilePath) {
                Debug.Log("Save path: " + Application.persistentDataPath);
            }
            FileStream file = File.Open(Application.persistentDataPath + "/save.dat", FileMode.OpenOrCreate);
            bf.Serialize(file, data);
            file.Close();
        }

        public static bool LoadGame<T>(out T data) {
            try {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream file = File.Open(Application.persistentDataPath + "/save.dat", FileMode.OpenOrCreate)) {
                    data = (T)bf.Deserialize(file);
                }
                return true;
            }
            catch (SerializationException e) {
                Debug.Log(e);
                data = default;
                return false;
            }
        }

    }
}