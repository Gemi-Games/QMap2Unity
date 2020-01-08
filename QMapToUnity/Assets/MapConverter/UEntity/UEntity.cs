using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QMapToUnity
{
    public delegate void TargetAction();

    [Serializable]
    public struct UKeyValue
    {
        public string Key;
        public string Value;

        public UKeyValue(string lKey, string lValue)
        {
            Key = lKey;
            Value = lValue;
        }
    }

    public class UEntity : MonoBehaviour
    {
        [HideInInspector, SerializeField]
        public string Classname;

        [SerializeField]
        public UKeyValue[] KeyValuePairs;

        [HideInInspector, SerializeField]
        public QBrush[] Brushes;

        [SerializeField]
        public Extents3D Extents;

        [HideInInspector, SerializeField]
        public GameObject[] BrushGOs;

        [HideInInspector, SerializeField]
        public bool HasAngle;
        [HideInInspector, SerializeField]
        public float Angle;

        [HideInInspector, SerializeField]
        public bool HasTarget;
        [HideInInspector, SerializeField]
        public string Target;

        [HideInInspector, SerializeField]
        public bool HasTargetname;
        [HideInInspector, SerializeField]
        public string Targetname;

        [HideInInspector, SerializeField]
        public int SpawnFlags;

        private event TargetAction m_OnTargeted;

        public event TargetAction OnTargeted
        {
            add { m_OnTargeted += value; }
            remove { m_OnTargeted -= value; }
        }

        public void ClearEvents()
        {
            m_OnTargeted = null;
        }

        public void SetupEntity(UEntity lUEnt)
        {
            QEntity lQEnt = ConvertToQEntity(lUEnt);

            SetupEntity(lQEnt);
        }

        public void SetupEntity(QEntity lQEnt)
        {
            AddData(lQEnt);

            HasAngle = false;
            HasTarget = false;
            HasTargetname = false;

            {
                float lAngle;

                if (GetValue("angle", out lAngle))
                    HasAngle = true;
                else
                    lAngle = 0f;

                Angle = lAngle;
            }

            {
                string lTarget;

                if (GetValue("target", out lTarget) && lTarget.Length > 0)
                {
                    HasTarget = true;
                    Target = lTarget;
                }
            }

            {
                string lTargetname;

                if (GetValue("targetname", out lTargetname) && lTargetname.Length > 0)
                {
                    HasTargetname = true;
                    Targetname = lTargetname;
                }
            }

            BrushGOs = GetChildrenObjects();

            if (!GetValue("spawnflags", out SpawnFlags))
                SpawnFlags = 0;

            SetupChildEntity();
        }

        private void AddData(QEntity lQEnt)
        {
            Classname = lQEnt.Classname;

            KeyValuePairs = new UKeyValue[lQEnt.KeyValuePairs.Length];

            for (int i = 0; i < lQEnt.KeyValuePairs.Length; i++)
            {
                string lKey = lQEnt.KeyValuePairs[i].Key;
                string lValue = lQEnt.KeyValuePairs[i].Value;

                KeyValuePairs[i] = new UKeyValue(lKey, lValue);
            }

            if (lQEnt.Brushes != null)
            {
                Brushes = new QBrush[lQEnt.Brushes.Length];

                for (int i = 0; i < lQEnt.Brushes.Length; i++)
                    Brushes[i] = lQEnt.Brushes[i];
            }
            else
                Brushes = new QBrush[0];
        }

        private QEntity ConvertToQEntity(UEntity lUEnt)
        {
            QEntity lQEnt = new QEntity();

            lQEnt.Classname = lUEnt.Classname;

            lQEnt.KeyValuePairs = new KeyValuePair<string, string>[lUEnt.KeyValuePairs.Length];

            for (int i = 0; i < lUEnt.KeyValuePairs.Length; i++)
            {
                string lKey = lUEnt.KeyValuePairs[i].Key;
                string lValue = lUEnt.KeyValuePairs[i].Value;

                lQEnt.KeyValuePairs[i] = new KeyValuePair<string, string>(lKey, lValue);
            }

            if (lUEnt.Brushes != null)
            {
                lQEnt.Brushes = new QBrush[lUEnt.Brushes.Length];

                for (int i = 0; i < lUEnt.Brushes.Length; i++)
                    lQEnt.Brushes[i] = lUEnt.Brushes[i];
            }

            return lQEnt;
        }

        private GameObject[] GetChildrenObjects()
        {
            List<GameObject> lTemp = new List<GameObject>();

            for (int i = 0; i < transform.childCount; i++)
                lTemp.Add(transform.GetChild(i).gameObject);

            return lTemp.ToArray();
        }

        public virtual void SetupChildEntity()
        {

        }

        public virtual void InitialiseEntity()
        {

        }

        public UEntity ReturnFirstTargetname(string lTarget)
        {
            return ReturnFirstTargetname(lTarget, UEntityManager.Instance.AllEntities);
        }

        public UEntity ReturnFirstTargetname(string lTarget, UEntity[] lUEnts)
        {
            for (int i = 0; i < lUEnts.Length; i++)
                if (lUEnts[i].Targetname == lTarget)
                    return lUEnts[i];

            return null;
        }

        public bool GetValue(string lKey, out string lValue)
        {
            for (int i = 0; i < KeyValuePairs.Length; i++)
            {
                if (KeyValuePairs[i].Key == lKey)
                {
                    lValue = KeyValuePairs[i].Value;
                    return true;
                }
            }

            lValue = "";

            return false;
        }

        public bool GetValue(string lKey, out float lValue)
        {
            string lString;

            if (GetValue(lKey, out lString))
            {
                lValue = float.Parse(lString);
                return true;
            }

            lValue = 0f;
            return false;
        }

        public bool GetValue(string lKey, out int lValue)
        {
            string lString;

            if (GetValue(lKey, out lString))
            {
                lValue = int.Parse(lString);
                return true;
            }

            lValue = 0;
            return false;
        }

        public bool GetValue(string lKey, out Vector3 lValue)
        {
            string lString;

            if (GetValue(lKey, out lString))
            {
                string[] lValues = lString.Split(' ');
                lValue = new Vector3(float.Parse(lValues[0]) / 16f, float.Parse(lValues[2]) / 16f, float.Parse(lValues[1]) / 16f);
                return true;
            }

            lValue = Vector3.zero;
            return false;
        }

        public bool GetValue(string lKey, out Color lValue)
        {
            string lString;

            if (GetValue(lKey, out lString))
            {
                string[] lValues = lString.Split(' ');
                lValue = new Color(float.Parse(lValues[0]) / 255f, float.Parse(lValues[1]) / 255f, float.Parse(lValues[2]) / 255f);
                return true;
            }

            lValue = Color.black;
            return false;
        }

        public void FireTarget()
        {
            if (!HasTarget)
                return;

            UEntity[] lUTargetnameEnts = UEntityManager.Instance.TargetnameEntities;
            int lLength = lUTargetnameEnts.Length;

            for (int i = 0; i < lLength; i++)
            {
                UEntity lEnt = lUTargetnameEnts[i];

                if (Target == lEnt.Targetname)
                    lEnt.m_OnTargeted?.Invoke();
            }
        }

        public IEnumerator FireTargetAfterDelay(float lDelay)
        {
            yield return new WaitForSeconds(lDelay);

            FireTarget();
        }

        private void OnDrawGizmos()
        {
            if (HasAngle)
                Debug.DrawLine(transform.position, transform.position + transform.forward * 2f, Color.white, 0f, true);
        }
    }
}