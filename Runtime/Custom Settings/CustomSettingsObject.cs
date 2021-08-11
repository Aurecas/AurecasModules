using System;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
namespace AurecasLib.Settings {

    public abstract class CustomSettingsObject : ScriptableObject {
        static Dictionary<Type, CustomSettingsObject> DefaultSettings = new Dictionary<Type, CustomSettingsObject>();

        public static T GetDefaultSettings<T>() where T : CustomSettingsObject {
            if (!DefaultSettings.ContainsKey(typeof(T))) {
                CustomSettingsAttribute attr = typeof(T).GetCustomAttribute(typeof(CustomSettingsAttribute)) as CustomSettingsAttribute;
                DefaultSettings.Add(typeof(T), Resources.Load(attr.defaultPath, typeof(T)) as T);
            }
            return DefaultSettings[typeof(T)] as T;
        }

#if UNITY_EDITOR
        public static CustomSettingsObject GetOrCreateSettings(string path, Type type) {
            var settings = AssetDatabase.LoadAssetAtPath("Assets/Resources/" + path + ".asset", type);
            if (settings == null) {
                settings = CreateInstance(type);
                AssetDatabase.CreateAsset(settings, "Assets/Resources/" + path + ".asset");
                AssetDatabase.SaveAssets();
            }
            if (DefaultSettings.ContainsKey(type)) {
                DefaultSettings[type] = settings as CustomSettingsObject;
            }
            else DefaultSettings.Add(type, settings as CustomSettingsObject);

            return settings as CustomSettingsObject;
        }

        public static SerializedObject GetSerializedSettings(string path, Type type) {
            return new SerializedObject(GetOrCreateSettings(path, type));
        }

#endif

    }
}
