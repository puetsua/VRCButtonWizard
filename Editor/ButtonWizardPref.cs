using UnityEditor;

namespace Puetsua.VRCButtonWizard.Editor
{
    public static class ButtonWizardPref
    {
        public static string SavePath
        {
            get => EditorPrefs.GetString(EditorPrefConst.SavePath);
            set => EditorPrefs.SetString(EditorPrefConst.SavePath, value);
        }

        public static SupportedLanguage Language
        {
            get => (SupportedLanguage) EditorPrefs.GetInt(EditorPrefConst.Language, (int) SupportedLanguage.English);
            set => EditorPrefs.SetInt(EditorPrefConst.Language, (int) value);
        }

        public static bool AlwaysAdvanced
        {
            get => EditorPrefs.GetBool(EditorPrefConst.AlwaysAdvanced, false);
            set => EditorPrefs.SetBool(EditorPrefConst.AlwaysAdvanced, value);
        }
    }
}