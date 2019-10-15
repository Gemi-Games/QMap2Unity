using UnityEngine;

namespace QMapToUnity
{
    public static class Q2UExtensions
    {
        public static int FindFirstLayerIndex(this LayerMask lLayer)
        {
            int lDecimal = 1;

            for (int i = 0; i < 32; i++)
            {
                if ((lDecimal & lLayer) == lDecimal)
                    return i;

                lDecimal *= 2;
            }

            return -1;
        }
    }
}
