using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AurecasLib.LevelSelection {
    public class DragAreaDetector : MonoBehaviour {

        bool pointerDown;
        bool dragging;

        public Action<Vector2, Vector2> OnDragBegin;
        public Action<Vector2, Vector2> OnDragEnded;
        public Action<Vector2, Vector2> OnDrag;

        void Update() {
            if (Input.touchCount > 0) {
                pointerDown = true;
            }

            if (pointerDown) {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved) {
                    if (!dragging) {
                        dragging = true;
                        DragStart();
                    }
                    else {
                        Drag();
                    }
                }

                if (touch.phase == TouchPhase.Ended) {
                    pointerDown = false;
                    if (dragging) {
                        dragging = false;
                        DragEnded();
                    }
                }
            }

        }

        public void DragStart() {
            OnDragBegin?.Invoke(Input.GetTouch(0).position, Input.GetTouch(0).deltaPosition);
        }

        public void DragEnded() {
            OnDragEnded?.Invoke(Input.GetTouch(0).position, Input.GetTouch(0).deltaPosition);
        }

        public void Drag() {
            OnDrag?.Invoke(Input.GetTouch(0).position, Input.GetTouch(0).deltaPosition);
        }
    }
}