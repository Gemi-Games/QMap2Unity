using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QMapToUnity
{
    public class ConvertMapWindow : EditorWindow
    {
        [SerializeField]
        private ConvertMapSettings m_Settings;

        [MenuItem("Q2U", menuItem = "Q2U/QMapToUnity Window")]
        public static void ShowWindow()
        {
            GetWindow<ConvertMapWindow>("Convert Map");
        }

        private void OnGUI()
        {
            minSize = new Vector2(320, 320);

            GUIStyle lLabelStyle = new GUIStyle();

            lLabelStyle.fontStyle = FontStyle.Bold;
            lLabelStyle.alignment = TextAnchor.MiddleLeft;
            lLabelStyle.margin = new RectOffset(8, 8, 3, 3);

            EditorGUI.LabelField(
                GUILayoutUtility.GetRect(GUIContent.none, lLabelStyle, GUILayout.Height(4)),
                "", lLabelStyle);

            //m_MapFile = (TextAsset)EditorGUI.ObjectField(
            //    GUILayoutUtility.GetRect(GUIContent.none, lLabelStyle, GUILayout.Height(16)),
            //    "Map file to convert", m_MapFile, typeof(TextAsset), false);


            //EditorGUI.LabelField(
            //    GUILayoutUtility.GetRect(GUIContent.none, lLabelStyle, GUILayout.Height(16)),
            //    "Definitions", lLabelStyle);

            m_Settings = (ConvertMapSettings)EditorGUI.ObjectField(
                GUILayoutUtility.GetRect(GUIContent.none, lLabelStyle, GUILayout.Height(16)),
                "Settings", m_Settings, typeof(ConvertMapSettings), false);

            //m_TexDefs = (TextureDefinitions)EditorGUI.ObjectField(
            //    GUILayoutUtility.GetRect(GUIContent.none, lLabelStyle, GUILayout.Height(16)),
            //    "Textures", m_TexDefs, typeof(TextureDefinitions), false);


            //EditorGUI.LabelField(
            //    GUILayoutUtility.GetRect(GUIContent.none, lLabelStyle, GUILayout.Height(16)),
            //    "Materials", lLabelStyle);

            //m_DefaultStandardMaterial = (Material)EditorGUI.ObjectField(
            //    GUILayoutUtility.GetRect(GUIContent.none, lLabelStyle, GUILayout.Height(16)),
            //    "Default", m_DefaultStandardMaterial, typeof(Material), false);

            //m_DefaultCutoutMaterial = (Material)EditorGUI.ObjectField(
            //    GUILayoutUtility.GetRect(GUIContent.none, lLabelStyle, GUILayout.Height(16)),
            //    "Cutout", m_DefaultCutoutMaterial, typeof(Material), false);

            //m_DefaultEmissiveMaterial = (Material)EditorGUI.ObjectField(
            //    GUILayoutUtility.GetRect(GUIContent.none, lLabelStyle, GUILayout.Height(16)),
            //    "Emissive", m_DefaultEmissiveMaterial, typeof(Material), false);

            //m_DefaultAreaLightMaterial = (Material)EditorGUI.ObjectField(
            //    GUILayoutUtility.GetRect(GUIContent.none, lLabelStyle, GUILayout.Height(16)),
            //    "Area light", m_DefaultAreaLightMaterial, typeof(Material), false);


            //EditorGUI.LabelField(
            //    GUILayoutUtility.GetRect(GUIContent.none, lLabelStyle, GUILayout.Height(16)),
            //    "Settings", lLabelStyle);

            //GUIStyle lBoolStyle = new GUIStyle();

            //lBoolStyle.margin = new RectOffset(8, 8, 4, 4);
            //lBoolStyle.padding = new RectOffset(2, 2, 0, 0);
            //lBoolStyle.alignment = TextAnchor.MiddleLeft;

            //m_AutoGenerateUV2 = EditorGUI.ToggleLeft(
            //    GUILayoutUtility.GetRect(GUIContent.none, lBoolStyle, GUILayout.Height(12)),
            //    "Auto generate UV2s", m_AutoGenerateUV2, lBoolStyle);

            //m_UsingHDRPMaterials = EditorGUI.ToggleLeft(
            //    GUILayoutUtility.GetRect(GUIContent.none, lBoolStyle, GUILayout.Height(12)),
            //    "Using HDRP Materials", m_UsingHDRPMaterials, lBoolStyle);

            ////m_SaveLevelAsAsset = EditorGUI.ToggleLeft(
            ////    GUILayoutUtility.GetRect(GUIContent.none, lBoolStyle, GUILayout.Height(12)),
            ////    "Save level as Asset", m_SaveLevelAsAsset, lBoolStyle);

            //m_SaveLevelAsAsset = false;

            GUIStyle lHelpStyle = new GUIStyle();

            lHelpStyle.fontStyle = FontStyle.Bold;
            lHelpStyle.alignment = TextAnchor.MiddleLeft;
            lHelpStyle.margin = new RectOffset(8, 8, 8, 8);


            if (m_Settings == null)
            {
                EditorGUI.HelpBox(GUILayoutUtility.GetRect(GUIContent.none, lHelpStyle, GUILayout.Height(40)), "Add a [ConvertMapSettings] scriptable object into the field above.", MessageType.None);
            }
            else if(m_Settings.MapFile == null)
            {
                EditorGUI.HelpBox(GUILayoutUtility.GetRect(GUIContent.none, lHelpStyle, GUILayout.Height(40)), "Missing [Map file]!", MessageType.Warning);
            }
            else if (m_Settings.EntDefs == null)
            {
                EditorGUI.HelpBox(GUILayoutUtility.GetRect(GUIContent.none, lHelpStyle, GUILayout.Height(40)), "Missing [Entity Definitions]!", MessageType.Warning);
            }
            else if (m_Settings.TexDefs == null)
            {
                EditorGUI.HelpBox(GUILayoutUtility.GetRect(GUIContent.none, lHelpStyle, GUILayout.Height(40)), "Missing [Texture Definitions]!", MessageType.Warning);
            }
            else if (m_Settings.StandardMaterial == null)
            {
                EditorGUI.HelpBox(GUILayoutUtility.GetRect(GUIContent.none, lHelpStyle, GUILayout.Height(40)), "Missing [Default Material]!", MessageType.Warning);
            }
            else if (m_Settings.EmissiveMaterial == null)
            {
                EditorGUI.HelpBox(GUILayoutUtility.GetRect(GUIContent.none, lHelpStyle, GUILayout.Height(40)), "Missing [Emissive Material]!", MessageType.Warning);
            }
            else if (m_Settings.AreaLightMaterial == null)
            {
                EditorGUI.HelpBox(GUILayoutUtility.GetRect(GUIContent.none, lHelpStyle, GUILayout.Height(40)), "Missing [Area Light Material]!", MessageType.Warning);
            }
            else if (m_Settings.CutoutMaterial == null)
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
                    ConvertMap.ConvertMapToUnityObjects(m_Settings);
                }
            }
        }
    }
}