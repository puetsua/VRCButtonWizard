using System;
using UnityEditor;
using UnityEngine;

namespace Puetsua.VRCButtonWizard.Editor
{
    public enum SupportedLanguage
    {
        [InspectorName("English")]
        English,
        [InspectorName("正體中文")]
        ChineseTraditional
    }

    public partial class LocalizedTextDataset
    {
        public static LocalizedTextDataset primary;

        static LocalizedTextDataset()
        {
            var selectedLanguage = (SupportedLanguage) EditorPrefs.GetInt(
                EditorPrefConst.Language, (int) SupportedLanguage.English);
            SetLanguage(selectedLanguage);
        }

        public string
            // Button Wizard
            buttonWizardWindowName,
            buttonWizardWindowTitle,
            buttonWizardSettingLabelLanguage,
            buttonWizardWindowLabelAvatar,
            buttonWizardWindowLabelTooltipAvatar,
            buttonWizardWindowMsgNoAvatar,
            buttonWizardWindowMenuAdvanced,
            // Advanced Button Wizard
            advancedButtonWizardWindowName,
            advancedButtonWizardWindowTitle,
            advancedButtonWizardWindowLabelSaveLocation,
            advancedButtonWizardWindowLabelTooltipSaveLocation,
            // Setting
            settingWindowLabelTitle,
            // Other
            creatorName;

        public static void SetLanguage(SupportedLanguage language)
        {
            switch (language)
            {
                case SupportedLanguage.English:
                    primary = English;
                    break;
                case SupportedLanguage.ChineseTraditional:
                    primary = ChineseTraditional;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(language), language, null);
            }
        }
    }
}