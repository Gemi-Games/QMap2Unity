using UnityEngine;

namespace QMapToUnity
{
    public class QMapLevel : MonoBehaviour
    {
        public Extents3D Extents;

        [HideInInspector]
        public EntityDefinitions EntDefs;
    }
}
