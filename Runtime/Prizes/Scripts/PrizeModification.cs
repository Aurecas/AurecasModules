using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AurecasLib.Prizes {
    public abstract class PrizeModification<T> where T : PrizeItem {
        public abstract void AddModifiedPrizeToInventory(T prize);
    }
}
