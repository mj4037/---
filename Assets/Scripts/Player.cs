using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, Damageable
{
    [SerializeField] int hp;
    [SerializeField, Min(0f)] float speed = 6.0f;
    [SerializeField, Min(0f)] float turnSpeed = 12.0f;
    [SerializeField] Transform cam;
    [SerializeField] Transform pickupRange;

    [Header("Animation")]
    [SerializeField] Animator animator;

    Rigidbody rb;
    Vector2 moveInput;

    static readonly int HashIsMove = Animator.StringToHash("IsMove");

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (GetComponent<PlayerCoreCable>() == null)
            gameObject.AddComponent<PlayerCoreCable>();

        if (cam == null && Camera.main != null)
            cam = Camera.main.transform;

        if (animator == null)
            animator = GetComponentInChildren<Animator>(); // 자식에 Animator가 있으면 자동으로 잡힘

        hp = 600;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void OnDisable()
    {
        moveInput = Vector2.zero;
        if (animator != null)
            animator.SetBool(HashIsMove, false);
    }

    void Update()
    {
        if (animator != null)
        {
            // 입력 크기(0~1). 약간의 흔들림 방지용 임계값
            bool isMove = moveInput.sqrMagnitude > 0.01f;
            animator.SetBool(HashIsMove, isMove);
        }
    }

    void FixedUpdate()
    {
        Vector3 dir;

        if (cam == null)
        {
            dir = new Vector3(moveInput.x, 0f, moveInput.y);
        }
        else
        {
            Vector3 forward = cam.forward;
            Vector3 right = cam.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            dir = right * moveInput.x + forward * moveInput.y;
        }

        if (dir.sqrMagnitude > 1f) dir.Normalize();

        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);

        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            Quaternion newRotation = Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newRotation);
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("TakeDamage!");

        hp -= damage;

        if (hp <= 0)
        {
            State.Publish(Condition.FINISH);
        }
    }
}