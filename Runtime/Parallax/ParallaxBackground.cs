using UnityEngine;
using UnityEngine.UI;

namespace AurecasLib.Parallax {
    public class ParallaxBackground : MonoBehaviour {

        [System.Serializable]
        public struct ParallaxLayer {
            public float factor;
            public Vector2 direction;
            public Vector2 offset;
        }

        Image[] parallaxLayers;

        public ParallaxLayer[] layerData;

        private void Start() {
            parallaxLayers = GetComponentsInChildren<Image>();

            foreach (Image i in parallaxLayers) {
                i.material = new Material(i.material);
            }
        }

        private void Update() {

            for (int i = 0; i < layerData.Length; i++) {
                parallaxLayers[i].material.SetFloat("_Factor", layerData[i].factor);
                parallaxLayers[i].material.SetVector("_Direction", layerData[i].direction);
                parallaxLayers[i].material.SetVector("_Offset", layerData[i].offset);
            }

        }
    }
}