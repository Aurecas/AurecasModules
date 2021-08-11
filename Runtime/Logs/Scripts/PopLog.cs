using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace AurecasLib.Logs {

    public class PopLog : MonoBehaviour {
        public static PopLog Instance { get; set; }

        public GameObject popPrefab;
        List<Pop> pops;
        public float popHeight = 30f;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                if (Instance != this) {
                    Destroy(gameObject);
                    return;
                }
            }

            pops = new List<Pop>();
        }

        public static void Log(object data) {
            if (Instance) {
                Instance._Log(data);
            }
        }

        public static void Log(object data, Color color) {
            if (Instance) {
                Instance._Log(data, color);
            }
        }

        private void _Log(object data) {
            _Log(data, Color.white);
        }

        private void _Log(object data, Color color) {
            if (Debug.isDebugBuild) {
                GameObject pop = Instantiate(popPrefab, transform);
                Pop p = pop.GetComponent<Pop>();
                p.data = data.ToString();
                p.color = color;
                pops.Add(p);
                Debug.Log(data, gameObject);
            }
        }

        private void Update() {
            for (int i = pops.Count - 1; i >= 0; i--) {
                pops[i].targetMax = new Vector2(0, -i * popHeight);
                pops[i].targetHeight = popHeight;
                if (pops[i].alpha < 0) {
                    Destroy(pops[i].gameObject);
                    pops.RemoveAt(i);
                }
            }
        }
    }
}