using UnityEngine;

namespace QMapToUnity
{
    public class QMapLevel : MonoBehaviour
    {
        [HideInInspector]
        public EntityDefinitions EntDefs;

        private void Awake()
        {
            UEntityManager.Instance.SetUp(EntDefs);
        }
    }
}
