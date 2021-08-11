using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AurecasLib.Saving {
    [Serializable]
    public class SaveGame {
        [Serializable]
        public class LevelData {
            public bool unlocked;
            public bool finished;
            public bool[] ranking; //3 rankings por padrão
            public float bestTime;

            public LevelData() {
                ranking = new bool[3];
            }
        }

        [Serializable]
        public class InventoryItem {
            public string itemId;
            public int amount;
        }

        public int currency;
        public float timePlayed;
        [JsonConverter(typeof(LevelDataConverter))]
        public ComposedLevelDataList levels;
        public List<InventoryItem> inventory;

        public int GetDefaultCurrencyAmount() {
            return currency;
        }

        public void AddToDefaultCurrency(int coins) {
            currency += coins;
        }

        public bool SpendDefaultCurrency(int amount) {
            if(currency >= amount) {
                currency -= amount;
                return true;
            }
            else {
                return false;
            }

        }

        public int GetTotalStarsCollected() {
            int count = 0;
            foreach (var l in levels.GetWorlds()) {
                foreach (LevelData ld in l.list) {
                    for (int i = 0; i < ld.ranking.Length; i++) {
                        if (ld.ranking[i]) {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        public int GetTotalStarsCollected(int world) {
            if (world >= levels.GetWorldCount()) return 0;
            int count = 0;

            foreach (LevelData ld in levels.GetWorlds()[world].list) {
                for (int i = 0; i < ld.ranking.Length; i++) {
                    if (ld.ranking[i]) {
                        count++;
                    }
                }
            }
            return count;
        }

        public void AddItemToInventory(string itemId, int amount) {
            inventory.Add(new InventoryItem() {
                itemId = itemId,
                amount = amount
            });
            RevalidateInventory();
        }

        public int GetItemAmount(string itemId) {
            InventoryItem item = SearchItem(itemId);
            if (item != null) return item.amount;
            else return 0;
        }

        public InventoryItem SearchItem(string itemId) {
            RevalidateInventory();
            foreach(InventoryItem item in inventory) {
                if (item.itemId == itemId) return item;
            }
            return null;
        }

        private void RevalidateInventory() {
            //Faz um merge em todos os items de ID igual;
            Dictionary<string, int> revalidatedInventory = new Dictionary<string, int>();
            foreach(InventoryItem item in inventory) {
                if (!revalidatedInventory.ContainsKey(item.itemId)) {
                    revalidatedInventory.Add(item.itemId, item.amount);
                }
                revalidatedInventory[item.itemId] += item.amount;
            }

            //Recria a lista de inventário só com os itens q tem amount > 0
            inventory = new List<InventoryItem>();
            foreach(string key in revalidatedInventory.Keys) {
                if(revalidatedInventory[key] > 0) {
                    inventory.Add(new InventoryItem() { 
                        itemId = key,
                        amount = revalidatedInventory[key]
                    });
                }
            }
        }

        public void Initialize() {
            if(inventory == null) {
                inventory = new List<InventoryItem>();
            }
            if (levels == null) {
                levels = new ComposedLevelDataList();
                levels.AddWorld();
                levels.AddLevel(0, new LevelData() { unlocked = true });
            }
        }

        public void SetLevelData(int world, int level, LevelData levelData) {
            Initialize();
            while (world >= levels.GetWorldCount()) {
                levels.AddWorld();
            }
            while (level >= levels.GetLevels(world).Count) {
                levels.AddLevel(world, new LevelData());
            }

            levels.SetLevel(world, level, levelData);
        }

        public LevelData GetLevelData(int world, int level) {
            Initialize();
            if (world >= levels.GetWorldCount()) return new LevelData();
            if (level >= levels.GetLevels(world).Count) return new LevelData();

            if (world == 0 && level == 0) {
                levels.GetLevel(world, level).unlocked = true;
            }
            if (levels.GetLevel(world, level).ranking == null) levels.GetLevel(world, level).ranking = new bool[3];
            return levels.GetLevel(world, level);
        }

        public void GetNextWorldAndLevel(out int world, out int level) {
            Initialize();
            int worldCount = levels.GetWorldCount();
            for (int i = worldCount - 1; i >= 0; i--) {
                int levelCount = levels.GetLevels(i).Count;
                for (int j = levelCount - 1; j >= 0; j--) {
                    LevelData ld = GetLevelData(i, j);
                    if (ld.unlocked && !ld.finished) {
                        world = i;
                        level = j;
                        return;
                    }
                }
            }
            world = 0;
            level = 0;
        }

        public override string ToString() {
            Initialize();
            return JsonConvert.SerializeObject(this);
        }

        public string ToJson() {
            return JsonConvert.SerializeObject(this, new SaveGameConverter());
        }

        public static SaveGame FromJson(string json) {
            return JsonConvert.DeserializeObject(json, typeof(SaveGame), new SaveGameConverter()) as SaveGame;
        }

        class SaveGameConverter : JsonConverter {
            public override bool CanConvert(Type objectType) {
                return objectType == typeof(SaveGame);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
                JObject obj = serializer.Deserialize(reader) as JObject;
                SaveGame sg = obj.ToObject<SaveGame>();
                return sg;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
                writer.WriteRaw(JsonConvert.SerializeObject(value));
            }
        }
    }
}