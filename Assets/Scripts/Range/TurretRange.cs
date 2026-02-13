using UnityEngine;

public class TurretRange : MonoBehaviour
{
    Turret turret;

    private void OnTriggerEnter(Collider other)
    {
        Unit unit = other.GetComponent<Unit>();

        if (unit != null)
        {
            turret.targetList.Add(unit);
        }
    }
}
