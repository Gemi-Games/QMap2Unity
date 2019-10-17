using QMapToUnity;
using UnityEngine;
using UnityEngine.AI;

public class TriggerJump : UEntity
{
    private Vector3 m_JumpVelocity;

    public override void InitialiseEntity()
    {
        string lTarget;

        if (GetValue("target", out lTarget))
        {
            UEntity lEnt = ReturnFirstTargetname(lTarget);

            Vector3 lToTarget = lEnt.transform.position - transform.position;
            lToTarget.y *= -1f;

            Vector3 lEndPos = lEnt.transform.position;

            RaycastHit lHit;

            if (Physics.Raycast(lEnt.transform.position, lToTarget.normalized, out lHit, lToTarget.magnitude, LayerMask.NameToLayer("Default"), QueryTriggerInteraction.Ignore))
            {
                lEndPos = lHit.point;
            }

            GameObject lGO = new GameObject("OffMeshLink");
            lGO.transform.parent = transform;
            lGO.transform.localPosition = Vector3.zero;

            GameObject lGO2 = new GameObject("OffMeshLinkChild");
            lGO2.transform.parent = lGO.transform;
            lGO2.transform.position = lEndPos;

            OffMeshLink lLink = lGO.AddComponent<OffMeshLink>();
            lLink.startTransform = lGO.transform;
            lLink.endTransform = lGO2.transform;
            lLink.autoUpdatePositions = true;
        }
    }

    private void Start()
    {
        for (int i = 0; i < BrushGOs.Length; i++)
        {
            OnTriggerEnterDispatcher lOnTrigger = BrushGOs[i].AddComponent<OnTriggerEnterDispatcher>();
            lOnTrigger.OnEnter += OnTriggerEnterEvent;
        }

        m_JumpVelocity = Vector3.zero;

        string lTarget;

        if (GetValue("target", out lTarget))
        {
            UEntity lEnt = UEntityManager.Instance.ReturnFirstTargetname(lTarget);

            if (lEnt != null)
            {
                Vector3 lDelta = lEnt.transform.position - transform.position;

                float lGravity = Mathf.Abs(Physics.gravity.y);

                m_JumpVelocity.y = Mathf.Sqrt(2f * lGravity * Mathf.Abs(lDelta.y));

                Vector3 lNorm = lDelta;

                lNorm.y = 0f;

                float lDistance = lNorm.magnitude;

                float lTime = m_JumpVelocity.y / lGravity;

                float lHorezSpeed = lTime > 0f ? lDistance / lTime : 0f;

                lNorm.Normalize();

                m_JumpVelocity += lNorm * lHorezSpeed;
            }
        }
    }

    private void OnEnable()
    {

    }

    private void OnTriggerEnterEvent(Collider lCollider)
    {
        InfoPlayerStart lPlayer = lCollider.gameObject.GetComponent<InfoPlayerStart>();

        if (lPlayer != null)
        {
            lPlayer.Velocity = m_JumpVelocity;
        }
    }
}
