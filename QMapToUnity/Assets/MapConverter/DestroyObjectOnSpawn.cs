using UnityEngine;

namespace QMapToUnity
{
    public class DestroyObjectOnSpawn : MonoBehaviour
    {
        void Start()
        {
            Destroy(gameObject);
        }
    }
}
