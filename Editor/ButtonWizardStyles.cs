using UnityEditor;
using UnityEngine;

namespace Puetsua.VRCButtonWizard.Editor
{
    public static class ButtonWizardStyles
    {
        public static readonly GUIStyle Box = new GUIStyle("HelpBox");
        public static readonly GUIStyle MultipleFields = new GUIStyle("Box");

        public static readonly GUIStyle LabelCenter = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter
        };
        public static readonly GUIStyle LabelRight = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleRight
        };
        public static readonly GUIStyle Title = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            alignment = TextAnchor.MiddleLeft,
            fixedHeight = 26.0f,
        };
    }
}