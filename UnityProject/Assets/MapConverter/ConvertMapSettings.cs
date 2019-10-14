using UnityEngine;

namespace QMapToUnity
{
    [CreateAssetMenu(fileName = "ConvertMapSettings", menuName = "Gemi/ConvertMapSettings")]
    public class ConvertMapSettings : ScriptableObject
    {
        public TextAsset MapFile;

        public TextureDefinitions TexDefs;

        public EntityDefinitions EntDefs;

        public Material StandardMaterial;

        public Material CutoutMaterial;

        public Material EmissiveMaterial;

        public Material AreaLightMaterial;

        public bool AutoGenerateUV2s;

        public bool UsingHDRPMaterials;

        public bool SaveLevelAsAsset;

        public bool AutoDetectTextures;
    }
}
