using UnityEngine;

public class NormalTurret : Turret
{
    private void Awake()
    {
        bulletPrefab = (Bullet)Resources.Load("NormalBollet");
    }
}
