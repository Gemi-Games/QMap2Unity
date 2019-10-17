using System;
using QMapToUnity;
using UnityEngine;

public class InfoPlayerStart : UEntity
{
    [Header("Components")]
    [SerializeField]
    private CharacterController m_CharController;

    [SerializeField]
    private Camera m_Camera;

    [Header("Input")]
    [SerializeField]
    private KeyCode m_MoveForwardKey = KeyCode.W;

    [SerializeField]
    private KeyCode m_MoveBackKey = KeyCode.S;

    [SerializeField]
    private KeyCode m_LeftStrafeKey = KeyCode.A;

    [SerializeField]
    private KeyCode m_RightStrafeKey = KeyCode.D;

    [SerializeField]
    private KeyCode m_JumpKey = KeyCode.Space;

    [SerializeField]
    private float m_MouseSensitivity = 2f;

    [Header("Physics")]
    [SerializeField]
    private float m_GroundAcceleration = 160f;

    [SerializeField]
    private float m_AirAcceleration = 10f;

    [SerializeField]
    private float m_GroundMaxSpeed = 20f;

    [SerializeField]
    private float m_AirMaxSpeed = 100f;

    [SerializeField]
    private float m_Friction = 8f;

    [SerializeField]
    private float m_JumpHeight = 3f;

    private bool m_Grounded;

    private bool m_JumpQueued;

    private float m_Yaw, m_Pitch;

    private Vector3 m_Velocity;

    public Vector3 Velocity
    {
        get { return m_Velocity; }
        set { m_Velocity = value; }
    }

    public override void InitialiseEntity()
    {
        Vector3 lToVector = transform.forward;
        lToVector.y = 0f;
        lToVector.Normalize();

        m_Yaw = Vector3.SignedAngle(Vector3.right, lToVector, Vector3.up) + 90f;
        m_Pitch = 0f;

        m_Grounded = false;
        m_JumpQueued = false;

        Velocity = Vector3.zero;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Velocity.y > 0.01f)
            m_Grounded = false;

        UpdateJump();

        UpdateMove();

        UpdateLook();
    }

    private void UpdateJump()
    {
        if (Input.GetKeyDown(m_JumpKey))
            m_JumpQueued = true;
        if (Input.GetKeyUp(m_JumpKey))
            m_JumpQueued = false;

        if (Velocity.y > -1f && m_Grounded && m_JumpQueued)
        {
            m_Grounded = false;
            m_JumpQueued = false;

            float lJumpImpulse = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * m_JumpHeight);

            m_Velocity.y = lJumpImpulse;
        }
    }

    private void UpdateMove()
    {
        Vector3 lInputDir = Vector3.zero;

        Vector3 lForward = Quaternion.Euler(0f, m_Yaw, 0f) * Vector3.forward;
        Vector3 lRight = new Vector3(lForward.z, 0f, -lForward.x);

        if (Input.GetKey(m_LeftStrafeKey))
            lInputDir -= lRight;
        if (Input.GetKey(m_RightStrafeKey))
            lInputDir += lRight;

        if (Input.GetKey(m_MoveForwardKey))
            lInputDir += lForward;
        if (Input.GetKey(m_MoveBackKey))
            lInputDir -= lForward;

        if (lInputDir.sqrMagnitude < 0.01f)
            lInputDir = Vector3.zero;
        else if (lInputDir.sqrMagnitude > 1f)
            lInputDir = lInputDir.normalized;

        Vector3 lVelHorizontal = Velocity;
        lVelHorizontal.y = 0f;

        float lAccel, lMaxSpeed;

        if (m_Grounded)
        {
            lVelHorizontal -= lVelHorizontal * m_Friction * Time.deltaTime;

            lAccel = m_GroundAcceleration;
            lMaxSpeed = m_GroundMaxSpeed;
        }
        else
        {
            lAccel = m_AirAcceleration;
            lMaxSpeed = m_AirMaxSpeed;
        }

        if (lVelHorizontal.sqrMagnitude < lMaxSpeed * lMaxSpeed)
            lVelHorizontal += lInputDir * lAccel * Time.deltaTime;

        m_Velocity.x = lVelHorizontal.x;
        m_Velocity.z = lVelHorizontal.z;

        m_Velocity.y += Physics.gravity.y * Time.deltaTime;

        Vector3 lDeltaMove = Velocity * Time.deltaTime;

        CollisionFlags lFlags = m_CharController.Move(lDeltaMove);

        if (Velocity.y < 0.01f &&
            (lFlags & CollisionFlags.Below) == CollisionFlags.Below)
        {
            m_Grounded = true;
            m_Velocity.y = 0f;
        }
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
}
