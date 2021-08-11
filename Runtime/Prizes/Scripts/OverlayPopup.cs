using AurecasLib.Prizes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayPopup : PopupWindow
{

    public Image prizeImage;

    public IEnumerator OpenPrize(PrizeItem prize) {
        prizeImage.sprite = prize.itemIcon;
        yield return new WaitForSecondsRealtime(2f);
        ClosePopup();
    }

}
