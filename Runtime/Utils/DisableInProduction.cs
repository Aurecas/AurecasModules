using UnityEngine;
namespace AurecasLib.Utils {
    public class DisableInProduction : MonoBehaviour {
        private void Awake() {
            if (!Debug.isDebugBuild) {
                gameObject.SetActive(false);
            }
        }
    }

}