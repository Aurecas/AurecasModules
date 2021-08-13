using UnityEngine;

namespace AurecasLib.Parallax {
    public class ParallaxElement : MonoBehaviour {
        public float factor = 0;
        public Vector2 direction;
        Vector2 originalPos;

        private void Start() {
            originalPos = transform.position;
        }

        private void LateUpdate() {
            Vector2 cameraPos = Camera.main.transform.position;
            Vector2 dif = (cameraPos - originalPos) * direction;
            transform.position = cameraPos - dif * factor;
        }
    }
}