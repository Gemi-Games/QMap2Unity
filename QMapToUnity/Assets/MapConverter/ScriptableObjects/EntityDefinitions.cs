using System;
using UnityEngine;

namespace QMapToUnity
{
    [CreateAssetMenu(fileName = "EntityDefinitions", menuName = "QMapToUnity/Entity Definitions")]
    public class EntityDefinitions : ScriptableObject
    {
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

            return Worldspawn;
        }
    }

    [Serializable]
    public struct EntDef
    {
        public string Classname;
        public GameObject Prefab;

        public bool IsStatic;

        [Header("Entity")]
        public LayerMask EntLayer;
        public bool HasConvexCollider;
        public bool IsConvexTrigger;

        [Header("Colliders")]
        public LayerMask ColLayer;
        public bool HasCollider;
        public bool IsTrigger;

        [Header("Meshes")]
        public LayerMask MeshLayer;
        public bool HasMesh;

        public EntDef(string lClassname)
        {
            Classname = lClassname;
            Prefab = null;

            IsStatic = true;

            EntLayer = 1;
            HasConvexCollider = false;
            IsConvexTrigger = false;

            ColLayer = 1;
            HasCollider = true;
            IsTrigger = false;

            MeshLayer = 1;
            HasMesh = true;
        }
    }
}