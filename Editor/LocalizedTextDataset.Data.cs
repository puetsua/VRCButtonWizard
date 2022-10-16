namespace Puetsua.VRCButtonWizard.Editor
{
    public partial class LocalizedTextDataset
    {
        private static readonly LocalizedTextDataset English = new LocalizedTextDataset
        {
            // Base
            baseWindowLabelAvatar = "Avatar",
            baseWindowLabelAnimator = "Animator",
            baseWindowLabelVrcTargetMenu = "Target Menu",
            baseWindowLabelVrcParameters = "Parameters",
            baseWindowLabelTargetObject = "Toggle Object",
            baseWindowLabelMenuName = "Control Name",
            baseWindowLabelParameterName = "Parameter Name",
            baseWindowLabelSaveParameter = "Save Parameter",
            baseWindowLabelDefaultValue = "Default Value",
            // -- Tooltip
            baseWindowTooltipAvatar = "VRChat Avatar with VRCAvatarDescriptor component.",
            baseWindowTooltipAnimator = "Usually the animator under FX layer.",
            baseWindowTooltipVrcTargetMenu = "Target menu to add your toggle.",
            baseWindowTooltipVrcParameters = "VRChat Expression Parameters to control your animator.",
            baseWindowTooltipTargetObject = "Target object to toggle.",
            baseWindowTooltipMenuName = "The name of control in your menu.",
            baseWindowTooltipParameterName = "The name of parameter in VRC Expression Parameters",
            baseWindowTooltipSaveParameter = "This parameter should be saved or not when changing avatars.",
            baseWindowTooltipDefaultValue = "Default value for the parameter when " +
                                            "avatar is loaded if not saved, or performing avatar reset.",
            // -- Messages
            baseWindowMsgNoAvatar = "Drag your avatar in the field to start adding toggles.",
            baseWindowMsgObjectNotUnderAvatar = "Target Object is not under avatar.",

            // Button Wizard
            buttonWizardWindowName = "Button Wizard",
            buttonWizardWindowTitle = "VRChat Button Wizard",
            buttonWizardSettingLabelLanguage = "Language",
            buttonWizardWindowMenuAdvanced = "Open Advanced Button Wizard",

            // Advanced Button Wizard
            advancedButtonWizardWindowName = "Advanced Button Wizard",
            advancedButtonWizardWindowTitle = "VRChat Advanced Button Wizard",
            advancedButtonWizardWindowLabelSaveLocation = "Save Location",
            advancedButtonWizardWindowLabelTooltipSaveLocation = "Save Location for AnimationClip asset.",

            // Setting
            settingWindowLabelTitle = "Settings",
            // Other
            creatorName = "Puetsua"
        };
        private static readonly LocalizedTextDataset ChineseTraditional = new LocalizedTextDataset
        {
            // Base
            baseWindowLabelAvatar = "角色",
            baseWindowLabelAnimator = "動畫器",
            baseWindowLabelVrcTargetMenu = "目標表情目錄",
            baseWindowLabelVrcParameters = "VRC表情參數表",
            baseWindowLabelTargetObject = "開關物件",
            baseWindowLabelMenuName = "開關名稱",
            baseWindowLabelParameterName = "參數名稱",
            baseWindowLabelSaveParameter = "儲存參數",
            baseWindowLabelDefaultValue = "預設值",
            // -- Tooltip
            baseWindowTooltipAvatar = "帶有VRCAvatarDescriptor元件的角色(Avatar)物件。",
            baseWindowTooltipAnimator = "Animator，通常是FX動畫層的動畫器。",
            baseWindowTooltipVrcTargetMenu = "VRC Expression Menu, 開關新增時要放置在哪個表情目錄下。",
            baseWindowTooltipVrcParameters = "VRC Expression Parameter, 驅動動畫器的VRChat的VRC表情參數表。",
            baseWindowTooltipTargetObject = "要設定開關的目標物件。",
            baseWindowTooltipMenuName = "目錄內的開關名稱，可以取中文",
            baseWindowTooltipParameterName = "VRC表情參數表內的變數名稱",
            baseWindowTooltipSaveParameter = "決定這個參數是否存檔，下次使用角色時會維持上個數值。",
            baseWindowTooltipDefaultValue = "變數的預設值，角色重置時或沒有存檔時會使用這個數值。",
            // -- Messages
            baseWindowMsgNoAvatar = "將角色(Avatar)代入開始建立開關。",
            baseWindowMsgObjectNotUnderAvatar = "開關物件不在角色物件底下。",

            // Button Wizard
            buttonWizardWindowName = "開關精靈",
            buttonWizardWindowTitle = "VRChat開關精靈",
            buttonWizardSettingLabelLanguage = "語言",
            buttonWizardWindowMenuAdvanced = "開啟進階版開關精靈",

            // Advanced Button Wizard
            advancedButtonWizardWindowName = "進階版開關精靈",
            advancedButtonWizardWindowTitle = "VRChat 進階版開關精靈",
            advancedButtonWizardWindowLabelSaveLocation = "儲存位置",
            advancedButtonWizardWindowLabelTooltipSaveLocation = "動畫檔的儲存位置",

            // Setting
            settingWindowLabelTitle = "設定",

            // Other
            creatorName = "飛蛇"
        };
    }
}