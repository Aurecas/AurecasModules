using AurecasLib.LevelSelection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectSample : MonoBehaviour
{
    public HorizontalCaroussel horizontalCaroussel;
    public int itemCount;

    private void Start() {
        horizontalCaroussel.Construct(itemCount);
    }
}
