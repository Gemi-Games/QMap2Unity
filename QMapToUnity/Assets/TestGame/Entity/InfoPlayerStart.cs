using QMapToUnity;
using UnityEngine;

public class InfoPlayerStart : UEntity
{
    [SerializeField]
    private CharacterController m_CharController;

    [SerializeField]
    private Camera m_Camera;

    [SerializeField]
    private float m_MouseSensitivity = 2f;

    [SerializeField]
    private float m_Acceleration = 160f;

    [SerializeField]
    private float m_MaxSpeed = 20f;

    [SerializeField]
    private float m_Friction = 8f;

    private float m_Yaw, m_Pitch;

    private Vector3 m_Velocity;

    public override void InitialiseEntity()
    {
        Vector3 lToVector = transform.forward;
        lToVector.y = 0f;
        lToVector.Normalize();

        m_Yaw = Vector3.SignedAngle(Vector3.right, lToVector, Vector3.up) + 90f;
        m_Pitch = 0f;

        m_Velocity = Vector3.zero;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        UpdateLook();

        UpdateMove();
    }

    private void UpdateLook()
    {
        m_Yaw += Input.GetAxisRaw("Mouse X") * m_MouseSensitivity;

        if (m_Yaw >= 360f)
            m_Yaw -= 360f;
        else if (m_Yaw < 0f)
            m_Yaw += 360f;

        m_Pitch += Input.GetAxisRaw("Mouse Y") * m_MouseSensitivity;

        m_Pitch = Mathf.Clamp(m_Pitch, -90f, 90f);

        m_Camera.transform.rotation = Quaternion.Euler(-m_Pitch, m_Yaw, 0f);
    }

    private void UpdateMove()
    {
        Vector3 lInputDir = Vector3.zero;

        Vector3 lForward = Quaternion.Euler(0f, m_Yaw, 0f) * Vector3.forward;
        Vector3 lRight = new Vector3(lForward.z, 0f, -lForward.x);

        if (Input.GetKey(KeyCode.A))
            lInputDir -= lRight;
        if (Input.GetKey(KeyCode.D))
            lInputDir += lRight;

        if (Input.GetKey(KeyCode.W))
            lInputDir += lForward;
        if (Input.GetKey(KeyCode.S))
            lInputDir -= lForward;

        if (lInputDir.sqrMagnitude < 0.01f)
            lInputDir = Vector3.zero;
        else if (lInputDir.sqrMagnitude > 1f)
            lInputDir = lInputDir.normalized;

        Vector3 lVelHorizontal = m_Velocity;
        lVelHorizontal.y = 0f;

        float lFrictionTime = m_Friction * Time.deltaTime;

        lVelHorizontal -= lVelHorizontal * lFrictionTime;

        lVelHorizontal += lInputDir * m_Acceleration * Time.deltaTime;

        if (lVelHorizontal.sqrMagnitude > m_MaxSpeed * m_MaxSpeed)
            lVelHorizontal = lVelHorizontal.normalized * m_MaxSpeed;

        m_Velocity.x = lVelHorizontal.x;
        m_Velocity.z = lVelHorizontal.z;

        m_Velocity.y += Physics.gravity.y * Time.deltaTime;

        Vector3 lDeltaMove = m_Velocity * Time.deltaTime;

        m_CharController.Move(lDeltaMove);
    }
}
