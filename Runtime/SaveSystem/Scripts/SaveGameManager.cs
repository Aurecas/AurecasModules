using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace AurecasLib.Saving {
    public sealed class SaveGameManager : MonoBehaviour {
        public static SaveGameManager Instance;

        private SaveGame saveGame;
        float lastTimeSaved;

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
        }

        public T GetSaveGame<T>() where T : SaveGame {
            if (saveGame == null) {
                LoadGame<T>();
            }
            return saveGame as T;
        }
        public void LoadGame<T>(Action<T> afterLoad = null) where T : SaveGame {
            if (!SaveSystem.LoadGame(out saveGame)) {
                ResetSave<T>();
            }

            afterLoad?.Invoke(saveGame as T);
        }

        public void SaveGame(Action afterSave = null) {
            saveGame.timePlayed += Time.time - lastTimeSaved;
            lastTimeSaved = Time.time;
            SaveSystem.SaveGame(saveGame);
            afterSave?.Invoke();
        }

        public void ResetSave<T>(Action afterReset = null) where T : SaveGame {
            saveGame = Activator.CreateInstance(typeof(T)) as T;
            saveGame.Initialize();
            SaveGame(afterReset);
        }
    }

}