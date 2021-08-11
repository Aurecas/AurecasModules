using UnityEngine;
using System;
using Firebase.Analytics;
using System.Linq;

namespace AurecasLib.Analytics {
    public class AnalyticsManager : MonoBehaviour {
        bool analyticsDebugText;
        public static AnalyticsManager Instance;

        public Action OnUpdateUserProperties;

        Action delayedAction;
        float lastUserPropertiesUpdatedTime;

        bool eventsDisabled;

        public void DisableEventSending() {
            eventsDisabled = true;
        }

        public void EnableEventSending() {
            eventsDisabled = false;
        }

        private void Awake() {
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
            analyticsDebugText = Debug.isDebugBuild;
        }

        public void UpdateUserProperties() {
            if (eventsDisabled) return;
            if (Time.time - lastUserPropertiesUpdatedTime > 5f) {
                OnUpdateUserProperties?.Invoke();
                //SaveGame saveGame = SaveGameManager.Instance.GetSaveGame();
                //if (saveGame == null) return;
                //ColorBallSettings settings = ColorBallSettings.GetDefaultSettings();
                //saveGame.GetNextWorldAndLevel(out int world, out int level);
                //FirebaseAnalytics.SetUserProperty("highest_world", world.ToString());
                //FirebaseAnalytics.SetUserProperty("highest_level", level.ToString());
                //FirebaseAnalytics.SetUserProperty("highest_level_name", settings.levelCollection.levels[world].GetLevelName(level));
                //FirebaseAnalytics.SetUserProperty("time_played", saveGame.timePlayed.ToString());
                //FirebaseAnalytics.SetUserProperty("currency", saveGame.GetCoins().ToString());
                //FirebaseAnalytics.SetUserProperty("skins_purchased", saveGame.GetOwnedSkinCount().ToString());
                //FirebaseAnalytics.SetUserProperty("total_ads_watched", saveGame.totalAdsWatched.ToString());
                //FirebaseAnalytics.SetUserProperty("total_stars", saveGame.GetTotalStarsCollected().ToString());
                //FirebaseAnalytics.SetUserProperty("changeset", settings.changeset.ToString());
                //FirebaseAnalytics.SetUserProperty("environment", Debug.isDebugBuild ? "development" : "production");
                lastUserPropertiesUpdatedTime = Time.time;
            }
        }

        private void Update() {
            if (delayedAction != null) {
                delayedAction.Invoke();
                delayedAction = null;
            }
        }

        private Parameter[] GetDefaultParameters() {
            return new Parameter[] {
            //Definir quais são os parametros padrão q vão ser enviados em todos os eventos
        };
        }

        public void SendEvent(string eventName, params Parameter[] parameters) {
            if (eventsDisabled) return;
            if (analyticsDebugText) {
                string t = "Event " + eventName;
                Debug.Log(t);
            }
            UpdateUserProperties();
            FirebaseAnalytics.LogEvent(eventName, parameters.Concat(GetDefaultParameters()).ToArray());
        }
    }
}