using QMapToUnity;
using UnityEngine;

public class LightSun : UEntity
{
    [SerializeField]
    private Light m_Light;

    private float m_Yaw, m_Pitch = 60f;

    public override void SetupChildEntity()
    {
        if (!GetValue("yaw", out m_Yaw))
            m_Yaw = 60f;

        if (!GetValue("pitch", out m_Pitch))
            m_Pitch = 60f;

        float lIntensity = 0f;
        if (GetValue("intensity", out lIntensity))
            m_Light.intensity = lIntensity;

        int lMode = 0;
        if (GetValue("lightMode", out lMode))
            m_Light.lightmapBakeType = (LightmapBakeType)lMode;
        if (GetValue("shadowMode", out lMode))
            m_Light.shadows = (LightShadows)lMode;

        Color lColor;
        if (GetValue("color", out lColor))
            m_Light.color = lColor;

        transform.rotation = Quaternion.Euler(m_Pitch, m_Yaw, 0f);
    }
}
