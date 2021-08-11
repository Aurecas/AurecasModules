using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AurecasLib.UI {
    public class TiledBackground : MonoBehaviour {
        // Parameters
        public string sortingLayer;
        public int sortingOrder;
        public UpdateType updateType;

        // Components
        Camera myCam;
        MeshFilter myMF;
        MeshRenderer myMR;

        // Internal
        List<Vector2> listUVs = new List<Vector2>(4);
        Vector2 imageScale;
        float lastCameraZoom;

        public enum UpdateType {
            Update,
            LateUpdate,
            FixedUpdate
        }

        private void FixedUpdate() {
            if (updateType == UpdateType.FixedUpdate) {
                Refresh();
            }
        }

        void LateUpdate() {
            if (updateType == UpdateType.LateUpdate) {
                Refresh();
            }
        }

        private void Update() {
            if (updateType == UpdateType.Update) {
                Refresh();
            }
        }

        public void Refresh() {
            // Recalculate if camera zoom chances
            float size = myCam.orthographicSize;
            if (size != lastCameraZoom) {
                lastCameraZoom = size;

                imageScale = new Vector2(size * myCam.aspect * 3f, size * 3f);
                transform.localScale = imageScale;
            }

            // Update UVs to simulate horizontal movement
            float sx = imageScale.x;
            float sy = imageScale.y;
            float x = transform.position.x - imageScale.x / 2f;
            float y = transform.position.y - imageScale.y / 2f;

            listUVs[0] = new Vector2(0f + x, 0f + y);
            listUVs[3] = new Vector2(sx + x, sy + y);
            listUVs[1] = new Vector2(sx + x, 0f + y);
            listUVs[2] = new Vector2(0f + x, sy + y);
            myMF.mesh.SetUVs(0, listUVs);
        }

        void Awake() {
            // Components
            myCam = GetComponentInParent<Camera>();
            myMF = GetComponent<MeshFilter>();
            myMR = GetComponent<MeshRenderer>();

            // Set sorting layer
            if (!string.IsNullOrWhiteSpace(sortingLayer)) {
                myMR.sortingLayerName = sortingLayer;
                myMR.sortingOrder = sortingOrder;
            }

            // Initial UVs
            listUVs.Add(new Vector2(0f, 0f));
            listUVs.Add(new Vector2(1f, 1f));
            listUVs.Add(new Vector2(1f, 0f));
            listUVs.Add(new Vector2(0f, 1f));
        }
    }
}