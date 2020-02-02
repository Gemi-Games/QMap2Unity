
using UnityEditor;
using UnityEngine;

namespace QMapToUnity
{
    [CustomEditor(typeof(ConvertMapSettings))]
    public class ConvertMapSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ConvertMapSettings lValue = (ConvertMapSettings)target;

            GUIStyle lHelpStyle = new GUIStyle();

            lHelpStyle.fontStyle = FontStyle.Bold;
            lHelpStyle.alignment = TextAnchor.MiddleLeft;
            lHelpStyle.margin = new RectOffset(8, 8, 8, 8);


            if (lValue == null)
            {
                EditorGUI.HelpBox(GUILayoutUtility.GetRect(GUIContent.none, lHelpStyle, GUILayout.Height(40)), "Add a [ConvertMapSettings] scriptable object into the field above.", MessageType.None);
            }
            else if (lValue.MapFile == null)
            {
                EditorGUI.HelpBox(GUILayoutUtility.GetRect(GUIContent.none, lHelpStyle, GUILayout.Height(40)), "Missing [Map file]!", MessageType.Warning);
            }
            else if (lValue.EntDefs == null)
            {
                EditorGUI.HelpBox(GUILayoutUtility.GetRect(GUIContent.none, lHelpStyle, GUILayout.Height(40)), "Missing [Entity Definitions]!", MessageType.Warning);
            }
            else if (lValue.TexDefs == null)
            {
                EditorGUI.HelpBox(GUILayoutUtility.GetRect(GUIContent.none, lHelpStyle, GUILayout.Height(40)), "Missing [Texture Definitions]!", MessageType.Warning);
            }
            else if (lValue.StandardMaterial == null)
            {
                EditorGUI.HelpBox(GUILayoutUtility.GetRect(GUIContent.none, lHelpStyle, GUILayout.Height(40)), "Missing [Default Material]!", MessageType.Warning);
            }
            else if (lValue.EmissiveMaterial == null)
            {
                EditorGUI.HelpBox(GUILayoutUtility.GetRect(GUIContent.none, lHelpStyle, GUILayout.Height(40)), "Missing [Emissive Material]!", MessageType.Warning);
            }
            else if (lValue.AreaLightMaterial == null)
            {
                EditorGUI.HelpBox(GUILayoutUtility.GetRect(GUIContent.none, lHelpStyle, GUILayout.Height(40)), "Missing [Area Light Material]!", MessageType.Warning);
            }
            else if (lValue.CutoutMaterial == null)
            {
                EditorGUI.HelpBox(GUILayoutUtility.GetRect(GUIContent.none, lHelpStyle, GUILayout.Height(40)), "Missing [Cutout Material]!", MessageType.Warning);
            }
            else
            {
                EditorGUI.HelpBox(GUILayoutUtility.GetRect(GUIContent.none, lHelpStyle, GUILayout.Height(40)), "All good to go!", MessageType.Info);

                GUIStyle lButtonStyle = new GUIStyle();

                lButtonStyle.fontStyle = FontStyle.Bold;
                lButtonStyle.alignment = TextAnchor.MiddleLeft;
                lButtonStyle.margin = new RectOffset(48, 48, 12, 12);

                if (GUI.Button(GUILayoutUtility.GetRect(GUIContent.none, lButtonStyle, GUILayout.Height(32)), "Convert Map"))
                {
                    ConvertMap.ConvertMapToUnityObjects(lValue);
                }
            }
        }
    }
}
