using UnityEngine;

public class UnitRange : MonoBehaviour
{
    [SerializeField] Unit unit;

    private void OnTriggerEnter(Collider other)
    {
        Damageable damageble = other.GetComponent<Damageable>();

        if (damageble != null)
        {
            Debug.Log("aaaaaaaaaaaaa Range");

            unit.Attack(damageble);
        }
    }
}
