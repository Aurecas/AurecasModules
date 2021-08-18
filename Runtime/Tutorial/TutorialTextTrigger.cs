using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AurecasLib.Tutorial {
    public class TutorialTextTrigger : MonoBehaviour {
        [System.NonSerialized] public TutorialText master;

        public string playerTag;

        void OnTriggerEnter2D(Collider2D collision) {
            if (collision.CompareTag(playerTag)) {
                master.OnEnter();
            }
        }

        void OnTriggerExit2D(Collider2D collision) {
            if (collision.CompareTag(playerTag)) {
                master.OnExit();
            }
        }
    }
}