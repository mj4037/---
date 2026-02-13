using Unity.VisualScripting;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float lifeTime;
    [SerializeField] public int baseDamage = 15;
    [SerializeField] public Transform turretHead;
    [SerializeField] public Turret turret;

    [SerializeField] Vector3 dir;

    float timer;

    public void Awake()
    {
        turretHead = turret.muzzle;
        turret = GetComponentInParent<Turret>();
        speed = turret.bulletSpeed;
        timer = 3f;
    }

    public void Init(Vector3 direction)
    {
        dir = direction.normalized;
        timer = 0f;
    }

    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit target = other.GetComponent<Unit>();

        if (target != null)
        {
            target.TakeDamage((int)((baseDamage + (turret.atk * 3)) * turret.atkMultiplier));

            Destroy(gameObject);
        }
    }
}
