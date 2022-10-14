namespace Puetsua.VRCButtonWizard.Editor
{
    public partial class LocalizedTextDataset
    {
        private static readonly LocalizedTextDataset English = new LocalizedTextDataset
        {
            // Button Wizard
            buttonWizardWindowName = "Button Wizard",
            buttonWizardWindowTitle = "VRChat Button Wizard",
            buttonWizardSettingLabelLanguage = "Language",
            buttonWizardWindowLabelAvatar = "Avatar",
            buttonWizardWindowLabelTooltipAvatar = "VRChat Avatar with VRCAvatarDescriptor component.",
            buttonWizardWindowMsgNoAvatar = "Drag your avatar in the field to start adding toggles.",
            buttonWizardWindowMenuAdvanced = "Open Advanced Button Wizard",

            // Advanced Button Wizard
            advancedButtonWizardWindowName = "Advanced Button Wizard",
            advancedButtonWizardWindowTitle = "VRChat Advanced Button Wizard",

            // Setting
            settingWindowLabelTitle = "Settings",
            // Other
            creatorName = "Puetsua"
        };
        private static readonly LocalizedTextDataset ChineseTraditional = new LocalizedTextDataset
        {
            // Button Wizard
            buttonWizardWindowName = "開關精靈",
            buttonWizardWindowTitle = "VRChat開關精靈",
            buttonWizardSettingLabelLanguage = "語言",
            buttonWizardWindowLabelAvatar = "角色",
            buttonWizardWindowLabelTooltipAvatar = "帶有VRCAvatarDescriptor元件的角色(Avatar)物件。",
            buttonWizardWindowMsgNoAvatar = "將角色(Avatar)代入開始建立開關。",
            buttonWizardWindowMenuAdvanced = "開啟進階版開關精靈",

            // Advanced Button Wizard
            advancedButtonWizardWindowName = "進階版開關精靈",
            advancedButtonWizardWindowTitle = "VRChat 進階版開關精靈",
            // Setting
            settingWindowLabelTitle = "設定",

            // Other
            creatorName = "飛蛇"
        };
    }
}