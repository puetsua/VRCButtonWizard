using UnityEditor;

namespace Puetsua.VRCButtonWizard.Editor
{
    public static class ButtonWizardPref
    {
        public static string SavePath
        {
            get => ProjectPrefs.GetString(ProjectPrefConst.SavePath);
            set => ProjectPrefs.SetString(ProjectPrefConst.SavePath, value);
        }

        public static SupportedLanguage Language
        {
            get => (SupportedLanguage) ProjectPrefs.GetInt(ProjectPrefConst.Language, (int) SupportedLanguage.English);
            set => ProjectPrefs.SetInt(ProjectPrefConst.Language, (int) value);
        }

        public static bool AlwaysAdvanced
        {
            get => ProjectPrefs.GetBool(ProjectPrefConst.AlwaysAdvanced, false);
            set => ProjectPrefs.SetBool(ProjectPrefConst.AlwaysAdvanced, value);
        }
    }
}