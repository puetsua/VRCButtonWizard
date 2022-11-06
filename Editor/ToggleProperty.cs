using UnityEditor;
using UnityEngine;

namespace Puetsua.VRCButtonWizard.Editor
{
    public class ToggleProperty
    {
        public readonly GameObject root;
        public readonly GameObject gameObject;
        public EditorCurveBinding binding;
        public object valueOn;
        public object valueOff;

        public ToggleProperty(GameObject gameObject, GameObject root)
        {
            this.root = root;
            this.gameObject = gameObject;

            // Default property is toggling active/inactive.
            var bindings = AnimationUtility.GetAnimatableBindings(gameObject, root);
            foreach (var b in bindings)
            {
                if (b.type == typeof(GameObject) &&
                    b.propertyName == PropertyNameConst.IsActive)
                {
                    binding = b;
                    break;
                }
            }

            valueOn = true;
            valueOff = false;
        }

        public object GetValue(bool isOn)
        {
            return isOn ? valueOn : valueOff;
        }
    }
}