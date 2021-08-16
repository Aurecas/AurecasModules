using UnityEngine;

namespace AurecasLib.Levels {
    [CreateAssetMenu(fileName = "World Collection", menuName = "AurecasLib/Worlds/World Collection")]
    public class WorldCollection : ScriptableObject {
        public WorldSO[] worlds;

        public bool GetNextLevel(int world, int level, out int nextWorld, out int nextLevel) {

            int nl = level + 1;
            int nw = world;

            if(nl >= worlds[world].levels.Count) {
                nl = 0;
                nw++;
            }

            if(nw >= worlds.Length) {
                nw = 0;
                nextWorld = nw;
                nextLevel = nl;
                return false;
            }

            nextWorld = nw;
            nextLevel = nl;
            return true;

        }
    }
}
