using System;

namespace AurecasLib.Settings {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CustomSettingsAttribute : Attribute {

        public string label;
        public string[] tags;
        public string defaultPath;

        public CustomSettingsAttribute(string label, string[] tags, string defaultPath) {
            this.label = label;
            this.tags = tags;
            this.defaultPath = defaultPath;
        }
    }
}