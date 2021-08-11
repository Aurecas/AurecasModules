using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AurecasLib.LevelSelection {
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class HorizontalCaroussel : MonoBehaviour {

        //Parameters
        public GameObject sectionPrefab;
        public Button LeftButton;
        public Button RightButton;
        public DragAreaDetector dragDetector;
        public bool snapToContent = true;
        public bool forceChildToFullWidth = false;
        public float scrollSensitivity = 1f;

        [Header("Objects")]
        public TextMeshProUGUI levelGroupText;

        public Action<int> OnChanged;

        //Internal
        int index;
        RectTransform rect;
        bool dragMode;
        float sectionWidth = 0;
        HorizontalLayoutGroup horizontalLayout;
        Vector2 currentDragPosition;


        private void Awake() {

            horizontalLayout = GetComponent<HorizontalLayoutGroup>();

            dragDetector.OnDrag = DragEvent;
            dragDetector.OnDragBegin = BeginDragEvent;
            dragDetector.OnDragEnded = EndDragEvent;

            if(LeftButton)
                LeftButton.onClick.AddListener(PreviousItem);
            if(RightButton)
                RightButton.onClick.AddListener(NextItem);
        }

        public void Construct(int sectionCount, Action<int, GameObject> foreachLoaded = null) {
            horizontalLayout = GetComponent<HorizontalLayoutGroup>();
            rect = GetComponent<RectTransform>();

            if (forceChildToFullWidth)
                sectionWidth = rect.rect.width;
            else
                sectionWidth = sectionPrefab.GetComponent<RectTransform>().rect.width + horizontalLayout.spacing;

            for (int i = 0; i < sectionCount; i++) {
                GameObject section = Instantiate(sectionPrefab, transform);
                RectTransform r = section.GetComponent<RectTransform>();
                if (forceChildToFullWidth) {
                    r.offsetMin = new Vector2(0, r.offsetMin.y);
                    r.offsetMax = new Vector2(rect.rect.width, r.offsetMax.y);
                }
                foreachLoaded?.Invoke(i, section);
            }
            UpdateArrows();
        }

        private void Update() {
            if (!snapToContent) {
                if (currentDragPosition.x > 0) currentDragPosition.x = 0;
                if (currentDragPosition.x < -(transform.childCount - 1) * sectionWidth) currentDragPosition.x = -(transform.childCount - 1) * sectionWidth;
            }

            if (dragMode || !snapToContent) {
                Vector2 r = rect.anchoredPosition;
                r += (currentDragPosition - r) / 5f * Time.deltaTime * 60f;
                rect.anchoredPosition = r;
            }
            else {
                float position = -index * sectionWidth;
                Vector2 target = new Vector3(position, rect.anchoredPosition.y);
                Vector2 r = rect.anchoredPosition;
                r += (target - r) / 5f * Time.deltaTime * 60f;
                rect.anchoredPosition = r;
            }

            //Drag
        }

        public void SetIndex(int idx, bool animate) {
            index = idx;
            if (index >= transform.childCount) {
                index = transform.childCount - 1;
            }
            if (index < 0) index = 0;

            if (!animate) {
                float position = -index * sectionWidth;
                rect.anchoredPosition = new Vector3(position, rect.anchoredPosition.y);
            }
            OnChanged?.Invoke(idx);
            UpdateArrows();
        }

        public void NextItem() {
            index++;
            if (index >= transform.childCount) {
                index = transform.childCount - 1;
            }
            UpdateArrows();
            OnChanged?.Invoke(index);
        }

        public void PreviousItem() {
            index--;
            if (index < 0) index = 0;
            UpdateArrows();
            OnChanged?.Invoke(index);
        }

        public void UpdateArrows() {
            if(LeftButton)
                LeftButton.interactable = index != 0;
            if(RightButton)
                RightButton.interactable = index != transform.childCount - 1;
        }

        public int GetSelectedValue() {
            return index;
        }

        public void LoadScene(string scene) {
            Transition.Transition.Goto(scene);
        }

        public void BeginDragEvent(Vector2 position, Vector2 deltaPosition) {
            dragMode = true;
            currentDragPosition = rect.anchoredPosition;
        }

        public void EndDragEvent(Vector2 position, Vector2 deltaPosition) {
            dragMode = false;
            if (!snapToContent) return;

            //se o delta > threshold, muda
            if (deltaPosition.x > 50) {
                PreviousItem();
                return;
            }
            if (deltaPosition.x < -50) {
                NextItem();
                return;
            }

            //Se moveu mais q a metade da tela, muda
            if (currentDragPosition.x - rect.anchoredPosition.x > .5f) {
                PreviousItem();
                return;
            }
            if (currentDragPosition.x - rect.anchoredPosition.x < -.5f) {
                NextItem();
                return;
            }
        }

        public void DragEvent(Vector2 position, Vector2 deltaPosition) {
            currentDragPosition.x += deltaPosition.x / 2f * scrollSensitivity;
        }

    }
}