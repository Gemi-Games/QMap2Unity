using System;
using UnityEngine;

namespace QMapToUnity
{
    [CreateAssetMenu(fileName = "EntityDefinitions", menuName = "QMapToUnity/Entity Definitions")]
    public class EntityDefinitions : ScriptableObject
    {
        public static EntDef NULL = new EntDef(null);

        public EntDef Worldspawn = new EntDef(MapParser.WORLDSPAWN);

        public EntDef[] Definitions;

        public EntDef GetDefinition(string lClassname)
        {
            if (lClassname == MapParser.WORLDSPAWN)
                return Worldspawn;
            else
                for (int i = 0; i < Definitions.Length; i++)
                    if (lClassname == Definitions[i].Classname)
                        return Definitions[i];

            Debug.LogWarning(lClassname + " definition could not be found! Defaulting to Worldspawn definition.");

            return NULL;
        }
    }

    [Serializable]
    public struct EntDef
    {
        public string Classname;
        public UEntity ConvertedPrefab;
        public UEntity RuntimePrefab;

        public bool IsStatic;

        [Header("Entity")]
        public SingleLayerMask EntLayer;
        public bool HasConvexCollider;
        public bool IsConvexTrigger;

        [Header("Colliders")]
        public SingleLayerMask ColLayer;
        public bool HasCollider;
        public bool IsTrigger;

        [Header("Meshes")]
        public SingleLayerMask MeshLayer;
        public bool HasMesh;

        public EntDef(string lClassname)
        {
            Classname = lClassname;

            ConvertedPrefab = null;
            RuntimePrefab = null;

            IsStatic = true;

            EntLayer = new SingleLayerMask(0);
            HasConvexCollider = false;
            IsConvexTrigger = false;

            ColLayer = new SingleLayerMask(0);
            HasCollider = true;
            IsTrigger = false;

            MeshLayer = new SingleLayerMask(0);
            HasMesh = true;
        }
    }
}