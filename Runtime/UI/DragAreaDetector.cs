using System;
using UnityEngine;

namespace AurecasLib.UI {
    public class DragAreaDetector : MonoBehaviour {

        bool pointerDown;
        bool dragging;

        public Action<Vector2, Vector2> OnDragBegin;
        public Action<Vector2, Vector2> OnDragEnded;
        public Action<Vector2, Vector2> OnDrag;

        Vector3 lastMousePos;

        void Update() {
            if (HasInputDown()) {
                pointerDown = true;
            }

            if (pointerDown) {
                Touch touch = GetTouch();
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

            lastMousePos = Input.mousePosition;

        }

        public bool HasInputDown() {
            return Input.touchCount > 0 || Input.GetMouseButton(0);
        }

        public Touch GetTouch() {
            if (Input.GetMouseButtonDown(0)) {
                Touch touch = new Touch() {
                    deltaTime = Time.deltaTime,
                    fingerId = 0,
                    position = Input.mousePosition,
                    deltaPosition = Input.mousePosition - lastMousePos,
                    phase = TouchPhase.Began,
                };
                return touch;
            }
            else if (Input.GetMouseButtonUp(0)) {
                Touch touch = new Touch() {
                    deltaTime = Time.deltaTime,
                    fingerId = 0,
                    position = Input.mousePosition,
                    deltaPosition = Input.mousePosition - lastMousePos,
                    phase = TouchPhase.Ended,
                };
                return touch;
            }
            else if (Input.GetMouseButton(0)) {
                Touch touch = new Touch() {
                    deltaTime = Time.deltaTime,
                    fingerId = 0,
                    position = Input.mousePosition,
                    deltaPosition = Input.mousePosition - lastMousePos,
                    phase = Input.mousePosition == lastMousePos ? TouchPhase.Stationary : TouchPhase.Moved,
                };
                return touch;
            }
            else {
                return Input.GetTouch(0);
            }
        }

        public void DragStart() {
            OnDragBegin?.Invoke(GetTouch().position, GetTouch().deltaPosition);
        }

        public void DragEnded() {
            OnDragEnded?.Invoke(GetTouch().position, GetTouch().deltaPosition);
        }

        public void Drag() {
            OnDrag?.Invoke(GetTouch().position, GetTouch().deltaPosition);
        }
    }
}