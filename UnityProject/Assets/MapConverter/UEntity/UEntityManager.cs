using System.Collections.Generic;
using UnityEngine;

namespace QMapToUnity
{
    public class UEntityManager : MonoBehaviour
    {
        private static UEntityManager s_Instance;

        public static UEntityManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    GameObject lSingleton = new GameObject();
                    s_Instance = lSingleton.AddComponent<UEntityManager>();
                    lSingleton.name = "(singleton) " + typeof(UEntityManager).ToString();
                }

                return s_Instance;
            }
        }

        public UEntity[] AllEntities { get; private set; }

        public Dictionary<string, UEntity[]> ClassnameEntityDictionary { get; private set; }

        public UEntity[] TargetnameEntities { get; private set; }

        protected UEntityManager() { }

        public void SetUp(EntityDefinitions lEntDefs)
        {
            ConvertAndSetupPrefabs(lEntDefs);

            AllEntities = FindObjectsOfType<UEntity>();

            for (int i = 0; i < AllEntities.Length; i++)
                AllEntities[i].InitialiseEntity();

            List<UEntity> lUTargetnameEnts = new List<UEntity>();

            Dictionary<string, List<UEntity>> lListDic = new Dictionary<string, List<UEntity>>();
            List<string> lListClassnames = new List<string>();

            for (int i = 0; i < AllEntities.Length; i++)
            {
                if (AllEntities[i].HasTargetname)
                    lUTargetnameEnts.Add(AllEntities[i]);

                string lClassname = AllEntities[i].Classname;

                if (!lListDic.ContainsKey(lClassname))
                {
                    List<UEntity> lNewList = new List<UEntity>();

                    lNewList.Add(AllEntities[i]);

                    lListClassnames.Add(lClassname);

                    lListDic.Add(lClassname, lNewList);
                }
                else
                {
                    lListDic[lClassname].Add(AllEntities[i]);
                }
            }

            TargetnameEntities = lUTargetnameEnts.ToArray();

            ClassnameEntityDictionary = new Dictionary<string, UEntity[]>(lListClassnames.Count);

            for (int i = 0; i < lListClassnames.Count; i++)
            {
                string lKey = lListClassnames[i];

                ClassnameEntityDictionary.Add(lKey, lListDic[lKey].ToArray());
            }
        }

        private void ConvertAndSetupPrefabs(EntityDefinitions lEntDefs)
        {
            AllEntities = FindObjectsOfType<UEntity>();

            for (int i = 0; i < AllEntities.Length; i++)
            {
                UEntity lOldUEnt = AllEntities[i];

                EntDef lEntDef = lEntDefs.GetDefinition(lOldUEnt.Classname);

                GameObject lNewObject = null;
                UEntity lNewUEnt = null;

                if (lEntDef.Prefab != null)
                {
                    lNewObject = GameObject.Instantiate(lEntDef.Prefab).gameObject;
                    lNewObject.name = i + " " + lOldUEnt.Classname;
                    lNewUEnt = lNewObject.GetComponent<UEntity>();
                }
                else
                {
                    lNewObject = new GameObject(i + " " + lOldUEnt.Classname);
                    lNewUEnt = lNewObject.AddComponent<UEmptyEntity>();
                }

                lNewObject.isStatic = lEntDef.IsStatic;
                lNewObject.layer = lEntDef.EntLayer.FindFirstLayerIndex();

                lNewObject.transform.parent = lOldUEnt.transform.parent;
                lNewObject.transform.position = lOldUEnt.transform.position;

                Transform[] lTranforms = new Transform[lOldUEnt.transform.childCount];

                for (int k = 0; k < lTranforms.Length; k++)
                    lTranforms[k] = lOldUEnt.transform.GetChild(k);

                for (int k = 0; k < lTranforms.Length; k++)
                    lTranforms[k].parent = lNewObject.transform;

                MeshCollider lCollider = lOldUEnt.GetComponent<MeshCollider>();

                if (lCollider != null)
                {
                    MeshCollider lNewCollider = lNewObject.AddComponent<MeshCollider>();
                    lNewCollider.sharedMesh = lCollider.sharedMesh;
                    lNewCollider.convex = true;
                    lNewCollider.isTrigger = lCollider.isTrigger;
                }

                lNewUEnt.SetupEntity(lOldUEnt);
            }

            for (int i = 0; i < AllEntities.Length; i++)
                DestroyImmediate(AllEntities[i].gameObject);
        }

        public void Clear()
        {
            AllEntities = null;
            TargetnameEntities = null;
        }

        public T ReturnFirstTargetname<T>(string lTarget) where T : UEntity
        {
            for (int i = 0; i < TargetnameEntities.Length; i++)
                if (TargetnameEntities[i].Targetname == lTarget)
                    return (T)TargetnameEntities[i];

            return null;
        }

        public UEntity ReturnFirstTargetname(string lTarget)
        {
            return ReturnFirstTargetname<UEntity>(lTarget);
        }
    }
}