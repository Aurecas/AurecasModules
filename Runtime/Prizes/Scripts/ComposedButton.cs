using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AurecasLib.Prizes {
    public class ComposedButton : MonoBehaviour {

        public GameObject adLoadingImage;
        public TextMeshProUGUI primaryTextUI;
        public TextMeshProUGUI secondaryTextUI;

        public string primaryText;
        public string secondaryText;

        Button button;
        bool rewardedLoaded = true;

        private void Start() {
            button = GetComponent<Button>();
        }

        public void SetRewardedLoaded(bool loaded) {
            rewardedLoaded = loaded;
        }

        private void Update() {
            if (rewardedLoaded) {
                if (secondaryTextUI) {
                    secondaryTextUI.SetText(secondaryText);
                }
            }
            else {
                if (adLoadingImage)
                    adLoadingImage.SetActive(true);
                if (secondaryTextUI) {
                    secondaryTextUI.SetText("Loading Ad");
                    secondaryTextUI.gameObject.SetActive(true);
                }
            }

            button.interactable = rewardedLoaded;
            if (adLoadingImage)
                adLoadingImage.SetActive(!rewardedLoaded);
        }
    }
}