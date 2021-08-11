#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
namespace AurecasLib.Editor {

    public class SceneInspectorSelector {
        public static string SceneSelector(string label, string scenePath, params GUILayoutOption[] options) {
            string[] allScenes = new string[EditorSceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < allScenes.Length; i++) {
                allScenes[i] = SceneUtility.GetScenePathByBuildIndex(i);
            }
            int idx = EditorGUILayout.Popup(label, Array.IndexOf(allScenes, scenePath), allScenes.Select((s) => CleanScenePathName(s)).ToArray(), options);
            if (idx >= 0 && idx < allScenes.Length) return allScenes[idx];
            return "";
        }

        public static string SceneSelector(string scenePath, params GUILayoutOption[] options) {
            string[] allScenes = new string[EditorSceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < allScenes.Length; i++) {
                allScenes[i] = SceneUtility.GetScenePathByBuildIndex(i);
            }
            int idx = EditorGUILayout.Popup(Array.IndexOf(allScenes, scenePath), allScenes.Select((s) => CleanScenePathName(s)).ToArray(), options);
            if (idx >= 0 && idx < allScenes.Length) return allScenes[idx];
            return "";
        }

        public static string SceneSelector(Rect rect, string label, string scenePath) {
            string[] allScenes = new string[EditorSceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < allScenes.Length; i++) {
                allScenes[i] = SceneUtility.GetScenePathByBuildIndex(i);
            }
            int idx = EditorGUI.Popup(rect, label, Array.IndexOf(allScenes, scenePath), allScenes.Select((s) => CleanScenePathName(s)).ToArray());
            if (idx >= 0 && idx < allScenes.Length) return allScenes[idx];
            return "";
        }

        public static string SceneSelector(Rect rect, string scenePath) {
            string[] allScenes = new string[EditorSceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < allScenes.Length; i++) {
                allScenes[i] = SceneUtility.GetScenePathByBuildIndex(i);
            }
            int idx = EditorGUI.Popup(rect, Array.IndexOf(allScenes, scenePath), allScenes.Select((s) => CleanScenePathName(s)).ToArray());
            if (idx >= 0 && idx < allScenes.Length) return allScenes[idx];
            return "";
        }

        static string CleanScenePathName(string path) {
            return path.Replace("Assets/Scenes/", "");
        }
    }
}
#endif
    