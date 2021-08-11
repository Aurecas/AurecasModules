using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AurecasLib.Logs {
    public class Pop : MonoBehaviour {

        public string data;
        public Vector2 targetMax;
        public float targetHeight;
        public float alpha;
        public Color color;

        Text text;
        RectTransform rect;

        // Start is called before the first frame update
        void Start() {
            text = GetComponent<Text>();
            rect = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update() {
            text.text = data;
            rect.offsetMax += (targetMax - rect.offsetMax) / 5f * Time.unscaledDeltaTime * 60f;
            rect.offsetMin += ((targetMax - new Vector2(500, targetHeight)) - rect.offsetMin) / 5f * Time.unscaledDeltaTime * 60f;
            alpha -= Time.unscaledDeltaTime;
            text.color = new Color(color.r, color.g, color.b, alpha); ;
        }
    }
}