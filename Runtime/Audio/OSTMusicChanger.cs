using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OSTMusicChanger : MonoBehaviour
{
    public AudioClip musicToChange;

    public void Start() {
        if (OSTManager.Instance) {
            OSTManager.Instance.PlayClip(musicToChange);
        }
    }
}
