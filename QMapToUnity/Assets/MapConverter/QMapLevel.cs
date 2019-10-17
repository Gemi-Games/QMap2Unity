using UnityEngine;

namespace QMapToUnity
{
    public class QMapLevel : MonoBehaviour
    {
        [HideInInspector]
        public EntityDefinitions EntDefs;

        private void Start()
        {
            UEntityManager.Instance.SetUp(EntDefs);
        }
    }
}
