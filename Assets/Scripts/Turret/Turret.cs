using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class Turret : MonoBehaviour
{
    [SerializeField] public float bulletSpeed;
    [SerializeField] public float atkMultiplier; // 공격력 배수
    [SerializeField] public int level;
    [SerializeField] public int range;
    [SerializeField] public int atk;
    [SerializeField] public float attackCooldown;

    [SerializeField] public List<Unit> targetList;
    [SerializeField] public Transform bulletPoint;
    [SerializeField] public Bullet bulletPrefab;
    [SerializeField] public Transform muzzle;

    public void Fire(Transform target)
    {
        Bullet b = Instantiate(bulletPrefab, muzzle.position, Quaternion.identity);
        Vector3 direction = (target.position - muzzle.position);
        b.Init(direction);
    }

    void OnMouseDown()
    {
        UpgradeUI ui = UpgradeUI.Instance;
        if (ui != null)
        {
            ui.Show(this);
        }
    }
}
