using UnityEngine;

namespace QMapToUnity
{
    [CreateAssetMenu(fileName = "ConvertMapSettings", menuName = "QMapToUnity/Convert Map Settings")]
    public class ConvertMapSettings : ScriptableObject
    {
        public TextAsset MapFile;

        [Header("Definitions")]
        public TextureDefinitions TexDefs;

        public EntityDefinitions EntDefs;

        [Header("Materials")]
        public Material StandardMaterial;

        public Material CutoutMaterial;

        public Material EmissiveMaterial;

        public Material AreaLightMaterial;

        [Header("Settings")]
        public bool AutoGenerateUV2s;

        public bool UsingHDRPMaterials;

        //public bool SaveLevelAsAsset;
    }
}
