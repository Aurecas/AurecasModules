using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using static AurecasLib.Saving.ComposedLevelDataList;
using static AurecasLib.Saving.SaveGame;

namespace AurecasLib.Saving {
    [Serializable]
    public class ComposedLevelDataList {
        public List<Wrapper> levels;

        [Serializable]
        public class Wrapper {
            public List<LevelData> list;
            public Wrapper() {
                list = new List<LevelData>();
            }
        }

        public ComposedLevelDataList() {
            levels = new List<Wrapper>();
        }

        public List<Wrapper> GetWorlds() {
            return levels;
        }

        public void Reset() {
            levels.Clear();
        }
        public void AddWorld() {
            levels.Add(new Wrapper());
        }

        public void AddLevel(int world, LevelData level) {
            levels[world].list.Add(level);
        }

        public void SetLevel(int world, int level, LevelData data) {
            levels[world].list[level] = data;
        }

        public List<LevelData> GetLevels(int world) {
            return levels[world].list;
        }

        public LevelData GetLevel(int world, int level) {
            return levels[world].list[level];
        }

        public int GetWorldCount() {
            return levels.Count;
        }

    }

    public class LevelDataConverter : JsonConverter {

        public override bool CanConvert(Type objectType) {
            return objectType == typeof(ComposedLevelDataList);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            ComposedLevelDataList cld = new ComposedLevelDataList();

            JArray arr = serializer.Deserialize(reader) as JArray;
            for (int i = 0; i < arr.Count; i++) {
                cld.AddWorld();
                JArray arr2 = arr[i].ToObject<JArray>();
                for (int j = 0; j < arr2.Count; j++) {
                    cld.AddLevel(i, arr2[j].ToObject<LevelData>());
                }
            }

            return cld;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            ComposedLevelDataList lst = value as ComposedLevelDataList;
            writer.WriteStartArray();
            foreach (Wrapper l in lst.GetWorlds()) {
                writer.WriteStartArray();
                foreach (LevelData level in l.list) {
                    JObject j = JObject.FromObject(level);
                    j.WriteTo(writer);
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }
    }

}