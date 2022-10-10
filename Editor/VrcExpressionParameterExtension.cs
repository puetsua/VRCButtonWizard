using System;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Puetsua.VRCButtonWizard.Editor
{
    internal static class VrcExpressionParameterExtension
    {
        internal static void AddToggle(this VRCExpressionParameters parameters,string parameter, bool isSaved, bool defaultValue)
        {
            var param = new VRCExpressionParameters.Parameter
            {
                name = parameter,
                valueType = VRCExpressionParameters.ValueType.Bool,
                saved = isSaved,
                defaultValue = defaultValue ? 1 : 0
            };
            parameters.AddParameter(param);
        }
        
        private static void AddParameter(this VRCExpressionParameters parameters,
            VRCExpressionParameters.Parameter parameter)
        {
            var newLength = parameters.parameters.Length + 1;
            var newParam = new VRCExpressionParameters.Parameter[newLength];
            Array.Copy(parameters.parameters, newParam, parameters.parameters.Length);
            newParam[newLength - 1] = parameter;
            parameters.parameters = newParam;
        }
    }
}