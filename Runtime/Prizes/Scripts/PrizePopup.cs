using AurecasLib.Popup;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace AurecasLib.Prizes {
    public class PrizePopup : PopupWindow {

        public bool isPrizeHidden;
        public bool isPrizeBoosted;
        public bool isPrizeFree;

        public Animator prizeHolderAnimator;
        public ComposedButton adButton, normalButton;
        public Image prizeImage;
        //public AssetReference overlayReference;

        public Action onPrizeClosed;
        private Action _winModified;

        PrizeItem currentPrize;
        bool prizeAnimationFinished;
        AssetReference overlayReference;
        
        public class PrizePopupParameters<T> where T : PrizeItem {
            public T prize;
            public bool isPrizeHidden;
            public bool isPrizeBoosted;
            public bool isPrizeFree;
            public PrizeModification<T> modification;
        }

        public void ClickAdButton() {
            //Mostra o ad

            StartCoroutine(PrizeAnimation());
        }

        public IEnumerator PrizeAnimation() {
            adButton.gameObject.SetActive(false);
            normalButton.gameObject.SetActive(false);
            if (isPrizeBoosted) {
                _winModified?.Invoke();
            }
            else {
                currentPrize.AddToInventory();
            }

            yield return null;

            if (isPrizeHidden) {
                //wait for prize animation
                prizeHolderAnimator.Play("Open", 0, 0);
                while (!prizeAnimationFinished) {
                    yield return new WaitForEndOfFrame();
                }
            }

            yield return PopupManager.Instance.OpenPopupRoutine(overlayReference, 100);
            OverlayPopup op = PopupManager.Instance.LoadedPopup as OverlayPopup;
            yield return op.OpenPrize(currentPrize);

            //wait for prize overlay
            ClosePopup();
            onPrizeClosed?.Invoke();

        }

        public void FinishPrizeOpenAnimator() {
            prizeAnimationFinished = true;
        }

        public void SetPrize<T>(PrizePopupParameters<T> parameters, AssetReference overlayReference) where T : PrizeItem{
            this.overlayReference = overlayReference;
            currentPrize = parameters.prize;
            this.isPrizeHidden = parameters.isPrizeHidden;
            this.isPrizeBoosted = parameters.isPrizeBoosted;
            this.isPrizeFree = parameters.isPrizeFree;
            prizeImage.sprite = parameters.prize.itemIcon;
            if (isPrizeFree) {
                adButton.gameObject.SetActive(false);
            }
            if (isPrizeHidden) {
                prizeHolderAnimator.Play("Hidden", 0, 0);
            }
            _winModified = () => {
                if(parameters.modification != null)
                    parameters.modification.AddModifiedPrizeToInventory(parameters.prize);
            };
        }
    
        public void ClickNormalButton() {
            //Ganha o premio normal, ou fecha o popup

            if (isPrizeBoosted || isPrizeFree) {
                StartCoroutine(PrizeAnimation());
            }
            else {
                ClosePopup();
                onPrizeClosed?.Invoke();
            }
        }

    }
}