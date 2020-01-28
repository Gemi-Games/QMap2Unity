using System;
using UnityEngine;

namespace QMapToUnity
{
    [Serializable]
    public struct Extents3D
    {
        public Vector3 Center;
        public Vector3 Size;

        public Vector3 Half;

        public Vector3 Min;
        public Vector3 Max;

        private bool m_Valid;

        public Extents3D(Vector3 lCenter, Vector3 lSize)
        {
            Center = lCenter;
            Size = lSize;

            Half = lSize / 2f;

            Min = lCenter - Half;
            Max = lCenter + Half;

            m_Valid = true;
        }

        public bool IsTouching(Vector3 lPos)
        {
            if (lPos.x < Min.x || lPos.x > Max.x ||
                lPos.z < Min.z || lPos.z > Max.z ||
                lPos.y < Min.y || lPos.y > Max.y )
                return false;
            else
                return true;
        }
    }
}