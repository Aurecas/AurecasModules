#if UNITY_EDITOR
#endif
using UnityEngine;
namespace AurecasLib.Settings {

    [CustomSettings("Template de Settings", new[] { "Tags", "De", "Pesquisa" }, "CaminhoDoAsset")]
    public class CustomSettingsTemplate : CustomSettingsObject {

        [Header("Primeira Aba")]
        public string minhaString;
        public int meuInt;

        [Header("Segunda Aba")]
        [Range(0, 100)]
        public float meuFloat;

        [SettingsButton("Método Legal", "Botoes (Terceira Aba)")]
        public void MetodoLegal() {
            Debug.Log("Método executado");
        }

        [SettingsButton("Outro Método Legal", "Primeira Aba")]
        public void OutroMetodoLegal() {
            Debug.Log("Outro Método executado");
        }

        //Não esqueça de trocar o tipo de "CustomSettingsTemplate" para o nome da sua classe
        public static CustomSettingsTemplate GetDefaultSettings() {
            return GetDefaultSettings<CustomSettingsTemplate>();
        }


        //Descomente isso pra que a aba apareça no Project Settings (não esqueça de trocar o tipo de "CustomSettingsTemplate" para o nome da sua classe)

        //#if UNITY_EDITOR
        //    [SettingsProvider]
        //    public static SettingsProvider CreateCustomSettingsProvider() {
        //        return CustomSettingsIMGUIRegister.CreateCustomSettingsProvider(typeof(CustomSettingsTemplate));
        //    }
        //#endif
    }

}