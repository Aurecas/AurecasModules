using UnityEngine;

namespace AurecasLib.Audio {
    public class OSTManager : MonoBehaviour {
        public static OSTManager Instance;

        AudioSource source;

        AudioClip bufferedClip;
        float bufferedPosition;

        float originalVolume;

        private void Awake() {
            source = GetComponent<AudioSource>();
            originalVolume = source.volume;

            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }
        }

        public void BufferOST() {
            bufferedClip = source.clip;
            bufferedPosition = source.time;
        }

        public void UnbufferOST() {
            if (bufferedClip != null) {
                source.clip = bufferedClip;
                source.Play();
                source.time = bufferedPosition;
            }
        }

        private void Update() {
            source.volume = PlayerPrefs.GetFloat("BGMVolume", 1) * originalVolume;
            if (PlayerPrefs.GetInt("BGMMute", 0) == 1) source.volume = 0;

            if (!IsPlaying()) {
                if (IsOnFirstSample()) {
                    source.Play();
                }
            }
        }

        public void SetLoop(bool loop) {
            source.loop = loop;
        }

        public void PlayClip(AudioClip clip) {
            if (clip != null && source.clip != clip) {
                source.clip = clip;
                source.time = 0;
                source.Play();
                Debug.Log("Music changed to " + clip.name);
            }
        }

        public bool IsPlaying() {
            return source.isPlaying;
        }

        public bool IsOnLastSample() {
            if (source.clip == null) return false;
            return source.timeSamples >= source.clip.samples;
        }

        public bool IsOnFirstSample() {
            return source.timeSamples == 0;
        }
    }
}