using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AurecasLib.Prizes {
    [CreateAssetMenu(fileName = "Item Database", menuName = "AurecasLib/Prizes/Item Database")]
    public class ItemDatabase : ScriptableObject {

        public List<PrizeItem> prizes;

        public T GetItemDefinition<T>(string itemId) where T : PrizeItem {
            foreach(PrizeItem item in prizes) {
                if(item.itemId == itemId) {
                    return item as T;
                }
            }
            Debug.LogError("Not item with id [" + itemId + "] was found on the database");
            return null;
        }

        public void RegisterPrize(PrizeItem prizeItem) {
            PrizeItem item = GetItemDefinition<PrizeItem>(prizeItem.itemId);

            if (item) {
                Debug.LogError("Trying to register item [" + prizeItem.itemId + "] witch is already registered: " + item);
            }
            else {
                prizes.Add(prizeItem);
            }
        }

    }
}
