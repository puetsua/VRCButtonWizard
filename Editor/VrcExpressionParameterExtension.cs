using System;
using UnityEngine;
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
            if (parameters.FindParameter(parameter.name) != null)
            {
                Debug.LogWarning($"Parameter '{parameter.name}' already exists in '{parameters.name}'.");
                return;
            }
            
            var newLength = parameters.parameters.Length + 1;
            var newParam = new VRCExpressionParameters.Parameter[newLength];
            Array.Copy(parameters.parameters, newParam, parameters.parameters.Length);
            newParam[newLength - 1] = parameter;
            parameters.parameters = newParam;
        }
    }
}