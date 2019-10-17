using QMapToUnity;
using UnityEngine;

public class LightPoint : UEntity
{
    [SerializeField]
    private Light m_Light;

    public override void SetupChildEntity()
    {
        float lTemp = 0f;
        if (GetValue("intensity", out lTemp))
            m_Light.intensity = lTemp;
        if (GetValue("range", out lTemp))
            m_Light.range = lTemp;

        int lMode = 0;
        if (GetValue("lightMode", out lMode))
            m_Light.lightmapBakeType = (LightmapBakeType)lMode;
        if (GetValue("shadowMode", out lMode))
            m_Light.shadows = (LightShadows)lMode;

        Color lColor;
        if (GetValue("color", out lColor))
            m_Light.color = lColor;
    }
}
