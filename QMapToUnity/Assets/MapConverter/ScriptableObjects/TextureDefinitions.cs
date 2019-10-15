using System;
using UnityEngine;

namespace QMapToUnity
{
    [CreateAssetMenu(fileName = "TextureDefinitions", menuName = "QMapToUnity/Texture Definitions")]
    public class TextureDefinitions : ScriptableObject
    {
        public Texture[] Textures;

        public TexDef[] Definitions;

        public bool HasDefinition(string lTextureName, out TexDef lTexDef)
        {
            for (int i = 0; i < Definitions.Length; i++)
                if (lTextureName == Definitions[i].Name)
                {
                    lTexDef = Definitions[i];
                    return true;
                }

            lTexDef = new TexDef();

            return false;
        }
    }

    [Serializable]
    public struct TexDef
    {
        [Header("Main")]
        public string Name;

        [Header("Cutout")]
        public bool IsCutout;
        [Range(0f, 1f)]
        public float CutoutAlpha;

        [Header("Emissive")]
        public Texture EmissiveTexture;
        [ColorUsage(false, true)]
        public Color EmissiveColour;

        [Header("Area Light")]
        public bool HasAreaLight;
        [ColorUsage(false, true)]
        public Color AreaColour;
        public float AreaDisplacement;
        public float AreaSizeScale;
    }
}
