using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using AurecasLib.Editor;
#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif

namespace AurecasLib.Levels {
    [CreateAssetMenu(fileName = "World", menuName = "AurecasLib/Worlds/World")]
    public class WorldSO : ScriptableObject {
        public string worldName;
        public Sprite worldCover;
        public Sprite worldBackground;
        public Color textColor;
        [HideInInspector]
        public List<string> levels;

        public string GetLevelName(int level) {
            string[] tokens = levels[level].Split('/');
            return tokens[tokens.Length - 2] + "/" + tokens[tokens.Length - 1].Replace(".unity", "");
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(WorldSO))]
    class LevelGroupEditor : UnityEditor.Editor {
        ReorderableList reorderableList;
        public void OnEnable() {
            List<string> allScenes = new List<string>();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
                allScenes.Add(SceneManager.GetSceneByBuildIndex(i).path);
            }
            WorldSO levelGroup = target as WorldSO;
            reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("levels"), true, true, true, true);
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                levelGroup.levels[index] = SceneInspectorSelector.SceneSelector(rect, "Level - " + (index + 1), levelGroup.levels[index]);
            };
        }
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            serializedObject.Update();
            reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Print level list json")) {
                WorldSO group = target as WorldSO;
                Debug.Log(JsonConvert.SerializeObject(group.levels, Formatting.Indented));
            }

            EditorUtility.SetDirty(target);
        }
    }
#endif
}