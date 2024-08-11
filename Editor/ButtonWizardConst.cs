using System.IO;
using UnityEngine;

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

                try
                {
                    string json = File.ReadAllText("Packages/vrchat.puetsuaworkshop.buttonwizard/package.json");
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    _cachedVersion = jsonObj.version;
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }

                return _cachedVersion;
            }
        }
    }
}