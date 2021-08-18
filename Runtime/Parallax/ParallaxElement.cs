using UnityEngine;

namespace AurecasLib.Parallax {
    public class ParallaxElement : MonoBehaviour {
        public float factor = 0;
        public Vector2 direction;
        public Vector2 offset;
        Vector2 originalPos;
        public UpdateMode updateMode;

        public enum UpdateMode {
            Normal,
            Fixed,
            Late
        }

        private void Start() {
            originalPos = transform.position;
        }

        public void UpdatePosition() {
            Vector2 cameraPos = Camera.main.transform.position;
            Vector2 dif = (cameraPos - originalPos) * direction;
            transform.position = cameraPos - dif * factor + offset;
        }

        private void Update() {
            if(updateMode == UpdateMode.Normal) {
                UpdatePosition();
            }
        }

        private void FixedUpdate() {
            if (updateMode == UpdateMode.Fixed) {
                UpdatePosition();
            }
        }

        private void LateUpdate() {
            if (updateMode == UpdateMode.Late) {
                UpdatePosition();
            }
        }
    }
}