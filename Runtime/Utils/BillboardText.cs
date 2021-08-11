using UnityEngine;

namespace AurecasLib.Utils {
    public class BillboardText : MonoBehaviour {
        TextMesh textMesh;

        private void Awake() {
            textMesh = GetComponent<TextMesh>();
        }

        public void SetText(string text) {
            if (textMesh)
                textMesh.text = text;
        }

        void Update() {
            transform.LookAt(Camera.main.transform);
        }
    }
}