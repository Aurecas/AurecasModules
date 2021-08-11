using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AurecasLib.UI {
    [ExecuteInEditMode]
    public class ProgressBar : MonoBehaviour {
        public Image fillBar;
        public float value;
        public float maxValue = 100;

        public FillMode fillMode;
        public Image.Type fillingImageType;

        public bool vertical = false;
        public Text textValue;
        public string textFormat;

        public enum FillMode {
            Fill,
            Stretch
        }

        public void SetValue(float value, float maxValue) {
            this.value = value;
            this.maxValue = maxValue;
        }

        // Update is called once per frame
        void Update() {
            float v = value / maxValue;

            switch (fillMode) {
                case FillMode.Stretch:
                    fillBar.type = fillingImageType;
                    RectTransform rectTransform = GetComponent<RectTransform>();
                    if (!vertical)
                        fillBar.rectTransform.offsetMax = new Vector2((v - 1) * rectTransform.rect.width, fillBar.rectTransform.offsetMax.y);
                    else
                        fillBar.rectTransform.offsetMax = new Vector2(fillBar.rectTransform.offsetMax.x, (v - 1) * rectTransform.rect.height);
                    break;
                case FillMode.Fill:
                    fillBar.type = Image.Type.Filled;
                    fillBar.fillMethod = vertical ? Image.FillMethod.Vertical : Image.FillMethod.Horizontal;
                    fillBar.fillAmount = v;
                    if (!vertical)
                        fillBar.rectTransform.offsetMax = new Vector2(0, fillBar.rectTransform.offsetMax.y);
                    else
                        fillBar.rectTransform.offsetMax = new Vector2(fillBar.rectTransform.offsetMax.x, 0);

                    break;
            }

            if (textValue != null) {
                textValue.text = string.Format(textFormat, value, maxValue);
            }
        }
    }
}