using UnityEngine;

public delegate void TriggerEvent(Collider lCollider);

public class OnTriggerEnterDispatcher : MonoBehaviour
{
    private event TriggerEvent m_OnEnter;

    public event TriggerEvent OnEnter
    {
        add { m_OnEnter += value; }
        remove { m_OnEnter -= value; }
    }

    private void OnTriggerEnter(Collider lCollider)
    {
        m_OnEnter?.Invoke(lCollider);
    }
}
