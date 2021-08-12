using UnityEngine;

namespace AurecasLib.Levels {
    [CreateAssetMenu(fileName = "World Collection", menuName = "AurecasLib/Worlds/World Collection")]
    public class WorldCollection : ScriptableObject {
        public WorldSO[] worlds;
    }
}
