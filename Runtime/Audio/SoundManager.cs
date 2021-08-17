using System.Collections.Generic;
using UnityEngine;

namespace AurecasLib.Audio {
    public class SoundManager : MonoBehaviour {
        public static SoundManager Instance;

        [System.Serializable]
        public class ClipData {
            public bool enabled = true;
            public AudioClip audioClip;
            [Range(0, 1)]
            public float volume = 1f;
        }

        Dictionary<string, ClipData> sfxLib;

        public void SetSFXLibrary(Dictionary<string, ClipData> lib) {
            sfxLib = lib;
        }

        void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else {
                if (Instance != this) {
                    Destroy(gameObject);
                    return;
                }
            }
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }

        AudioSource audioSource;

        public static void Play3DSFX(string key, Transform transform) {
            Play3DSFX(key, transform, 1);
        }

        public static void Play3DSFX(string key, Transform transform, float volume) {
            float dist = (Camera.main.transform.position - transform.position).magnitude;
            float factor = Mathf.Min(1, (1 / dist));
            PlaySFX(key, factor * volume);
        }

        public static void PlaySFX(string key, float volume) {
            Dictionary<string, ClipData> sfxLib = Instance.sfxLib;

            if (sfxLib.ContainsKey(key)) {
                ClipData cd = sfxLib[key];
                if (cd.enabled) {
                    if (cd.audioClip) {
                        Instance.audioSource.PlayOneShot(cd.audioClip, cd.volume * volume);
                    }
                    else {
                        Debug.LogWarning("Trying to play SFX " + key + " that has AudioClip null");
                    }
                }
            }
            else {
                Debug.LogWarning("Trying to play SFX " + key + " that is not registered");
            }
        }
        public static void PlaySFX(string key) {
            PlaySFX(key, 1);
        }

        public void UpdateVolume() {
            float vol = PlayerPrefs.GetFloat("SFXVolume", 1);
            if (PlayerPrefs.GetInt("SFXMute", 0) == 1) {
                vol = 0;
            }
            audioSource.volume = vol;
        }

        private void Start() {
            UpdateVolume();
        }


    }
}