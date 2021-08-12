using AurecasLib.UI;
using UnityEngine;

public class LevelSelectSample : MonoBehaviour {
    public HorizontalCaroussel horizontalCaroussel;
    public int itemCount;

    private void Start() {
        horizontalCaroussel.Construct(itemCount);
    }
}
