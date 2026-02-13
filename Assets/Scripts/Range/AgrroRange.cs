using UnityEngine;

public class AggroRange : MonoBehaviour
{
    [SerializeField] Unit unit;
    [SerializeField] Damageable target;   

    private void Awake()
    {
        unit = transform.parent.GetComponent<Unit>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Damageable damageable = other.GetComponent<Damageable>();

        if (damageable != null)
        {
            unit.ChangeTarget(other.transform);

            target = damageable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Damageable damageable = other.GetComponent<Damageable>();

        if (damageable == target)
        {
            unit.ChangeTarget(unit.core.transform);

            target = unit.core;
        }
    }
}
