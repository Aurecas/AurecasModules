using System;
using UnityEngine;

public class SwipeDetection : MonoBehaviour {

    public float minSwipeDistance = 1;
    public float maxSwipeTime = 1;

    Vector2 touchBegin;
    float touchTimestamp;
    public Action<Vector2> OnSwiped;

    private void Update() {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) {
                touchBegin = touch.position;
                touchTimestamp = Time.unscaledTime;
            }

            if(touch.phase == TouchPhase.Ended) {
                if(Time.unscaledTime - touchTimestamp < maxSwipeTime) {
                    if((touchBegin - touch.position).magnitude > minSwipeDistance) {
                        Swipe(touch.position - touchBegin);
                    }
                }
            }
        }
    }

    public void Swipe(Vector2 direction) {
        OnSwiped?.Invoke(direction);
    }
}
