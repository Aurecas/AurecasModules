using AurecasLib.Settings;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace AurecasLib.Editor {

    public static class CustomSettingsIMGUIRegister {
        static Dictionary<Type, int> selectedHeader = new Dictionary<Type, int>();

#if UNITY_EDITOR
        public static SettingsProvider CreateCustomSettingsProvider(Type type) {
            CustomSettingsAttribute st = type.GetCustomAttribute(typeof(CustomSettingsAttribute), true) as CustomSettingsAttribute;

            var provider = new SettingsProvider("Project/" + st.label, SettingsScope.Project) {
                label = st.label,
                guiHandler = (searchContext) => {
                    OnSettingsGUI(type, st);
                },
                keywords = new HashSet<string>(st.tags)
            };

            return provider;
        }


        struct FieldType {
            public bool isMethod;
            public string name;
            public string methodName;
        }

        static void OnSettingsGUI(Type type, CustomSettingsAttribute attr) {
            if (!typeof(CustomSettingsObject).IsAssignableFrom(type)) return;

            SerializedObject settings = CustomSettingsObject.GetSerializedSettings(attr.defaultPath, type);
            CustomSettingsObject customSettings = settings.targetObject as CustomSettingsObject;

            List<string> headerNames = new List<string>();
            Dictionary<int, List<FieldType>> headers = new Dictionary<int, List<FieldType>>();

            int currentHeader = 0;
            foreach (FieldInfo field in type.GetFields()) {
                HeaderAttribute header = field.GetCustomAttribute(typeof(HeaderAttribute)) as HeaderAttribute;
                if (header != null) {
                    if (!headerNames.Contains(header.header)) {
                        headerNames.Add(header.header);
                    }
                    currentHeader = Array.IndexOf(headerNames.ToArray(), header.header);
                }

                if (!headers.ContainsKey(currentHeader)) {
                    headers.Add(currentHeader, new List<FieldType>());
                }
                headers[currentHeader].Add(new FieldType() {
                    isMethod = false,
                    name = field.Name
                });
            }

            foreach (MethodInfo method in type.GetMethods()) {
                SettingsButtonAttribute settingsButton = method.GetCustomAttribute(typeof(SettingsButtonAttribute)) as SettingsButtonAttribute;
                if (settingsButton != null) {
                    string header = settingsButton.headerContainer;
                    if (!headerNames.Contains(header)) {
                        headerNames.Add(header);
                    }
                    currentHeader = Array.IndexOf(headerNames.ToArray(), header);

                    if (!headers.ContainsKey(currentHeader)) {
                        headers.Add(currentHeader, new List<FieldType>());
                    }
                    headers[currentHeader].Add(new FieldType() {
                        isMethod = true,
                        name = settingsButton.contextTitle,
                        methodName = method.Name
                    });
                }
            }

            if (!selectedHeader.ContainsKey(type)) {
                selectedHeader.Add(type, 0);
            }


            selectedHeader[type] = GUILayout.Toolbar(selectedHeader[type], headerNames.ToArray());

            if (headers.ContainsKey(selectedHeader[type])) {
                foreach (FieldType field in headers[selectedHeader[type]]) {

                    if (!field.isMethod) {
                        SerializedProperty serProp = settings.FindProperty(field.name);
                        if (serProp != null)
                            EditorGUILayout.PropertyField(serProp);
                    }
                    else {
                        if (GUILayout.Button(field.name)) {
                            try {
                                type.InvokeMember(field.methodName, BindingFlags.InvokeMethod, null, customSettings, new object[] { });
                            }
                            catch (Exception e) {
                                Debug.LogError("Tried to invoke method " + field.methodName + " from " + type.Name + ", but failed");
                                Debug.LogError(e);
                            }
                        }
                    }
                }
            }

            settings.ApplyModifiedProperties();
        }
#endif
    }
}