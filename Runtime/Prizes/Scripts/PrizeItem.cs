using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AurecasLib.Prizes {
    public abstract class PrizeItem : ScriptableObject {

        public string itemId;
        public Sprite itemIcon;

        public abstract void AddToInventory();
    }
}

