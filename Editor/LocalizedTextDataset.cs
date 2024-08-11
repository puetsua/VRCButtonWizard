using System;
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
            var selectedLanguage = ButtonWizardPref.Language;
            SetLanguage(selectedLanguage);
        }

        public string
            // Base
            baseWindowLabelAvatar,
            baseWindowLabelAnimatorController,
            baseWindowLabelAnimator,
            baseWindowLabelVrcTargetMenu,
            baseWindowLabelVrcParameters,
            baseWindowLabelTargetObject,
            baseWindowLabelMenuName,
            baseWindowLabelParameterName,
            baseWindowLabelSaveParameter,
            baseWindowLabelDefaultValue,
            baseWindowLabelInvertToggle,
            baseWindowButtonCreateToggle,
            baseWindowTipDropObjectsHere,
            baseWindowTipAnimatorRequired,
            // -- Tooltip
            baseWindowTooltipAvatar,
            baseWindowTooltipAnimatorController,
            baseWindowTooltipAnimator,
            baseWindowTooltipVrcTargetMenu,
            baseWindowTooltipVrcParameters,
            baseWindowTooltipTargetObject,
            baseWindowTooltipMenuName,
            baseWindowTooltipParameterName,
            baseWindowTooltipSaveParameter,
            baseWindowTooltipDefaultValue,
            baseWindowTooltipInverseToggle,
            // -- Messages
            baseWindowMsgNoAvatar,
            baseWindowMsgObjectNotUnderAvatar,
            // -- WindowMenu
            baseWindowMenuToggleAlwaysAdvanced,

            // Button Wizard
            buttonWizardWindowName,
            buttonWizardWindowTitle,
            buttonWizardSettingLabelLanguage,
            buttonWizardWindowMenuAdvanced,

            // Advanced Button Wizard
            advancedButtonWizardWindowName,
            advancedButtonWizardWindowTitle,
            advancedButtonWizardWindowLabelSaveLocation,
            advancedButtonWizardWindowLabelTooltipSaveLocation,
            advancedButtonWizardWindowMenuSimple,
            advancedButtonWizardWindowButtonCreateVrcToggle,
            advancedButtonWizardWindowButtonCreateAnimationClips,
            advancedButtonWizardWindowButtonCreateAnimatorControllerLayer,
            advancedButtonWizardWindowButtonCreateLayersAndClips,

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