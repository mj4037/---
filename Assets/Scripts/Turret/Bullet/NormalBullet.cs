using UnityEngine;

public class NormalBullet : Bullet
{
    [SerializeField] Turret Turret;

    void Awake()
    {
        base.Awake();
    }
}
