﻿using System;
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

            Debug.LogWarning(lClassname + " definition could not be found! Defaulting to Worldspawn settings.");

            EntDef lEntDef = new EntDef(Worldspawn, null, true);

            return lEntDef;
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

        public EntDef(EntDef lOriginal, string lNewClassname, bool lNullPrefabs = false)
        {
            Classname = lNewClassname;

            if (lNullPrefabs)
            {
                ConvertedPrefab = null;
                RuntimePrefab = null;
            }
            else
            {
                ConvertedPrefab = lOriginal.ConvertedPrefab;
                RuntimePrefab = lOriginal.RuntimePrefab;
            }

            IsStatic = lOriginal.IsStatic;

            EntLayer = lOriginal.EntLayer;
            HasConvexCollider = lOriginal.HasConvexCollider;
            IsConvexTrigger = lOriginal.IsConvexTrigger;

            ColLayer = lOriginal.ColLayer;
            HasCollider = lOriginal.HasCollider;
            IsTrigger = lOriginal.IsTrigger;

            MeshLayer = lOriginal.MeshLayer;
            HasMesh = lOriginal.HasMesh;
        }
    }
}