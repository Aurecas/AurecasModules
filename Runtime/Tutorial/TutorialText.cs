using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AurecasLib.Tutorial {
    public class TutorialText : MonoBehaviour {
        // Types
        public enum DisplayMode {
            AlwaysOn, ShowOnEnter, ShowAndHide
        }

        // Parameters
        [Header("Text")]
        [SerializeField] DisplayMode displayMode;
        [SerializeField] float delayToShow, delayToHide;
        [SerializeField] bool freezeOnShow;
        [SerializeField] float freezeTimeout = 1;
        [SerializeField] string animatorTriggerName = "Hide";
        [SerializeField] string playerTag = "Player";

        [Header("Enter Trigger")]
        [SerializeField] float delayToEnter;
        [SerializeField] UnityEvent enterEvent;

        [Header("Exit Trigger")]
        [SerializeField] float delayToExit;
        [SerializeField] UnityEvent exitEvent;

        // Components
        Animator myAnimator;
        TutorialTextTrigger myTrigger;

        // Internal
        bool isHiding, isShowing, isEntering, isExiting;
        bool freezeSkipped;

        public void OnEnter() {
            if (displayMode != DisplayMode.AlwaysOn) {
                if (delayToShow > 0) {
                    if (!isShowing) {
                        isShowing = true;
                        Debug.Log("OnEnter");
                        myTrigger.StartCoroutine(ShowDelayed());
                    }
                }
                else {
                    Debug.Log("OnEnter");
                    myTrigger.StartCoroutine(ShowDelayed());
                }
            }

            if (enterEvent != null) {
                if (delayToEnter > 0) {
                    if (!isEntering) {
                        isEntering = true;
                        myTrigger.StartCoroutine(EnterDelayed());
                    }
                }
                else {
                    myTrigger.StartCoroutine(EnterDelayed());
                }
            }
        }

        public void OnExit() {
            if (displayMode == DisplayMode.ShowAndHide) {
                if (delayToHide > 0) {
                    if (!isHiding) {
                        isHiding = true;
                        myTrigger.StartCoroutine(HideDelayed());
                    }
                }
                else {
                    myTrigger.StartCoroutine(HideDelayed());
                }
            }

            if (exitEvent != null) {
                if (delayToExit > 0) {
                    if (!isExiting) {
                        isExiting = true;
                        myTrigger.StartCoroutine(ExitDelayed());
                    }
                }
                else {
                    myTrigger.StartCoroutine(ExitDelayed());
                }
            }
        }

        public void DisableText() {
            gameObject.SetActive(false);
        }

        IEnumerator ShowDelayed() {

            yield return new WaitForSeconds(delayToShow);
            gameObject.SetActive(true);
            isShowing = false;

            if (freezeOnShow) {
                Debug.Log("Freeze time start");
                float lastTimeScale = Time.timeScale;
                if (freezeOnShow) {
                    Time.timeScale = 0;
                }

                float t = 0;
                while (t < freezeTimeout) {
                    t += Time.unscaledDeltaTime;
                    yield return null;
                    if (freezeSkipped) {
                        Debug.Log("Freeze Skipped");
                        break;
                    }
                }
                freezeSkipped = false;
                Time.timeScale = lastTimeScale;
                Debug.Log("Freeze finished, back to time scale: " + Time.timeScale);
            }
        }

        IEnumerator HideDelayed() {
            yield return new WaitForSeconds(delayToHide);

            myAnimator.SetTrigger(animatorTriggerName);
            isHiding = false;
        }

        IEnumerator EnterDelayed() {
            yield return new WaitForSeconds(delayToEnter);
            enterEvent.Invoke();
            isEntering = false;
        }

        IEnumerator ExitDelayed() {
            yield return new WaitForSeconds(delayToExit);
            exitEvent.Invoke();
            isExiting = false;
        }

        void OnDestroy() {
            if (myTrigger) {
                Destroy(myTrigger);
            }
        }

        private void Update() {
            if (Input.touchCount > 0) {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began) {
                    freezeSkipped = true;
                }
            }
            if (Input.GetMouseButtonDown(0)) {
                freezeSkipped = true;
            }
        }

        void Start() {
            var coll = GetComponent<BoxCollider2D>();

            // Create trigger object
            if (displayMode != DisplayMode.AlwaysOn || enterEvent != null || exitEvent != null) {

                GameObject trigger = new GameObject($"{name} Trigger", typeof(TutorialTextTrigger));
                trigger.transform.SetParent(transform.parent);
                trigger.transform.position = transform.position;
                trigger.transform.rotation = transform.rotation;
                trigger.transform.localScale = transform.localScale;


                myTrigger = trigger.GetComponent<TutorialTextTrigger>();
                myTrigger.master = this;
                myTrigger.playerTag = playerTag;
                if (coll) {
                    trigger.AddComponent<BoxCollider2D>();

                    var triggerColl = trigger.GetComponent<BoxCollider2D>();
                    triggerColl.size = coll.size;
                    triggerColl.offset = coll.offset;
                    triggerColl.isTrigger = true;
                }

                gameObject.SetActive(false);

            }

            // Remove trigger
            if (coll != null) Destroy(coll);
        }

        void Awake() {
            myAnimator = GetComponent<Animator>();
        }
    }
}