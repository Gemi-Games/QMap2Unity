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

    }

    public override void InitialiseEntity()
    {
        transform.rotation = Quaternion.Euler(m_Pitch, m_Yaw, 0f);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
