namespace Puetsua.VRCButtonWizard.Editor
{
    public static class ButtonWizardConst
    {
        public const int MajorVersion = 0;
        public const int MinorVersion = 3;
        public const int PatchVersion = 0;
        public const string PreRelease = "";

        public const int MenuItemPriority = 1123;

        public static readonly string VersionCore = $"{MajorVersion}.{MinorVersion}.{PatchVersion}";

        public static string Version => string.IsNullOrWhiteSpace(PreRelease)
            ? VersionCore
            : $"{VersionCore}-{PreRelease}";
    }
}