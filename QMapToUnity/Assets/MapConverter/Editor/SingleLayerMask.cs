using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SingleLayerMask))]
public class SingleLayerMaskPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect lPosition, SerializedProperty lProperty, GUIContent lLabel)
    {
        EditorGUI.BeginProperty(lPosition, GUIContent.none, lProperty);

        SerializedProperty layerIndex = lProperty.FindPropertyRelative("m_LayerIndex");

        lPosition = EditorGUI.PrefixLabel(lPosition, GUIUtility.GetControlID(FocusType.Passive), lLabel);

        if (layerIndex != null)
        {
            layerIndex.intValue = EditorGUI.LayerField(lPosition, layerIndex.intValue);
        }

        EditorGUI.EndProperty();
    }
}
