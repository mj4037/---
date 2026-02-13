using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [SerializeField] protected float speed;
    [SerializeField] protected float rotateSpeed = 720f;
    [SerializeField] protected int atk;
    [SerializeField] protected int hp;
    [SerializeField] public Unit ParentPrefab;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] public Core core;
    [SerializeField] public Player player;
    [SerializeField] public Transform target;

    bool warnedNoSpeed;
    bool warnedNoRb;




    [SerializeField] bool rotateToMoveDir = true;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        ResolveReferences();

        rotateSpeed = 720f;
    }

    private void OnEnable()
    {
        warnedNoSpeed = false;
        warnedNoRb = false;
        ResolveReferences();

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.WakeUp();
        }

        if (GameManager.Instance != null && GameManager.Instance.Wave != null)
        {
            hp = (int)(hp * (1 + (GameManager.Instance.Wave.wave - 1) * 0.12f));
            speed = speed * (1 + (GameManager.Instance.Wave.wave - 1) * 0.03f);
        }
    }

    void ResolveReferences()
    {
        if (GameManager.Instance != null)
        {
            if (core == null) core = GameManager.Instance.Core;
            if (player == null) player = GameManager.Instance.player;
        }

        if (core == null)
            core = FindObjectOfType<Core>();

        if (target == null && core != null)
            target = core.transform;
    }

    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void Move()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                if (!warnedNoRb)
                {
                    warnedNoRb = true;
                    Debug.LogWarning($"[{name}] Rigidbody missing. Unit will not move.");
                }
                return;
            }
        }

        if (target == null)
        {
            ResolveReferences();
            if (target == null) return;
        }

        if (speed <= 0f)
        {
            if (!warnedNoSpeed)
            {
                warnedNoSpeed = true;
                Debug.LogWarning($"[{name}] speed is {speed}. Unit will not move.");
            }
            return;
        }

        Vector3 to = target.position - rb.position;
        to.y = 0f;

        if (to.sqrMagnitude < 0.0001f) return;

        Vector3 dir = to.normalized;
        Vector3 vel = rb.linearVelocity;
        vel.x = dir.x * speed;
        vel.z = dir.z * speed;
        rb.linearVelocity = vel;

        if (rotateToMoveDir)
        {
            Quaternion goal = Quaternion.LookRotation(dir, Vector3.up);
            Quaternion rot = Quaternion.RotateTowards(rb.rotation, goal, rotateSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(rot);
        }
    }

    public void Attack(Damageable damageable)
    {
        Debug.Log("Attack");

        damageable.TakeDamage(15 + (atk * 3));

        Die();
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        GameManager.Instance.Spawn.Release(ParentPrefab, this);

        ChangeTarget(core.transform);
    }

    void FixedUpdate()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            if (rb.isKinematic)
                rb.isKinematic = false;

            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.WakeUp();
        }

        Move();
    }
}
