using System.IO;

namespace Puetsua.VRCButtonWizard.Editor
{
    public static class ButtonWizardConst
    {
        public const int MenuItemPriority = 1123;

        private static string _cachedVersion = "dev";

        public static string Version
        {
            get
            {
                if (_cachedVersion != "dev")
                    return _cachedVersion;

                // Read json file
                string json = File.ReadAllText("Packages/vrchat.puetsuaworkshop.buttonwizard/package.json");
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                if (jsonObj != null)
                {
                    _cachedVersion = jsonObj.version;
                }

                return _cachedVersion;
            }
        }
    }
}