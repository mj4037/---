using UnityEngine;

public class PlayerCoreCable : MonoBehaviour
{
    [SerializeField, Min(0f)] float coreCableRange = 5f;
    [SerializeField, Min(0f)] float coreCableWidth = 0.12f;
    [SerializeField, Min(1)] int coreCableSegments = 12;
    [SerializeField, Min(0f)] float coreCableSag = 1.2f;
    [SerializeField] LayerMask coreCableGroundLayers = ~0;
    [SerializeField, Min(0f)] float coreCableGroundClearance = 0.05f;
    [SerializeField] Color coreCableColor = new Color(0.2f, 0.8f, 1f, 1f);
    [SerializeField] Vector3 coreCablePlayerOffset = new Vector3(0f, 1.2f, 0f);
    [SerializeField] Vector3 coreCableCoreOffset = new Vector3(0f, 1.2f, 0f);
    [SerializeField, Min(0f)] float coreCableEndClearance = 0.05f;

    [SerializeField, Min(0f)] float turretCableReleaseRange = 3f;
    [SerializeField] LayerMask turretCableReleaseLayers = ~0;

    LineRenderer coreCable;
    bool isCoreCableLatched;
    Transform latchedCore;

    void Awake()
    {
        coreCable = GetComponent<LineRenderer>();
        if (coreCable == null)
            coreCable = gameObject.AddComponent<LineRenderer>();

        coreCable.enabled = false;
        coreCable.positionCount = Mathf.Max(2, coreCableSegments + 1);
        coreCable.useWorldSpace = true;
        coreCable.startWidth = coreCableWidth;
        coreCable.endWidth = coreCableWidth;
        coreCable.startColor = coreCableColor;
        coreCable.endColor = coreCableColor;
        if (coreCable.material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");

            if (shader != null)
            {
                coreCable.material = new Material(shader);
                if (coreCable.material.HasProperty("_BaseColor"))
                    coreCable.material.SetColor("_BaseColor", coreCableColor);
                else if (coreCable.material.HasProperty("_Color"))
                    coreCable.material.SetColor("_Color", coreCableColor);
            }
        }
    }

    void OnDisable()
    {
        isCoreCableLatched = false;
        latchedCore = null;
        if (coreCable != null)
            coreCable.enabled = false;
    }

    void Update()
    {
        if (coreCable == null)
            return;

        Core core = GameManager.Instance != null ? GameManager.Instance.Core : null;
        if (!isCoreCableLatched)
        {
            if (core != null)
            {
                bool inRange = Vector3.SqrMagnitude(transform.position - core.transform.position) <= coreCableRange * coreCableRange;
                if (inRange)
                {
                    TurretCoreCable.DeactivateActive();
                    isCoreCableLatched = true;
                    latchedCore = core.transform;
                }
            }
        }

        if (isCoreCableLatched)
        {
            bool nearTurret = false;
            Turret nearestTurret = null;
            float nearestTurretSqr = float.PositiveInfinity;

            Collider[] hits = Physics.OverlapSphere(transform.position, turretCableReleaseRange, turretCableReleaseLayers, QueryTriggerInteraction.Collide);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] == null) continue;
                Turret t = hits[i].GetComponentInParent<Turret>();
                if (t != null)
                {
                    nearTurret = true;
                    float d = Vector3.SqrMagnitude(transform.position - t.transform.position);
                    if (d < nearestTurretSqr)
                    {
                        nearestTurretSqr = d;
                        nearestTurret = t;
                    }
                }
            }

            if (nearTurret)
            {
                if (nearestTurret != null)
                {
                    TurretCoreCable turretCable = nearestTurret.GetComponent<TurretCoreCable>();
                    if (turretCable == null)
                        turretCable = nearestTurret.gameObject.AddComponent<TurretCoreCable>();

                    Transform coreT = latchedCore != null ? latchedCore : (core != null ? core.transform : null);
                    turretCable.ConnectToCore(coreT);
                }

                isCoreCableLatched = false;
                latchedCore = null;
                coreCable.enabled = false;
            }
            else
            {
                Transform coreT = latchedCore != null ? latchedCore : (core != null ? core.transform : null);
                if (coreT != null)
                {
                    Vector3 a = transform.position + coreCablePlayerOffset;
                    Vector3 b = coreT.position + coreCableCoreOffset;
                    Core coreComp = coreT.GetComponent<Core>();
                    Collider coreCol = coreComp != null ? coreComp.GetComponent<Collider>() : coreT.GetComponent<Collider>();
                    if (coreCol != null)
                    {
                        Vector3 closest = coreCol.ClosestPoint(a);
                        Vector3 toPlayer = a - closest;
                        if (toPlayer.sqrMagnitude > 0.0001f)
                            closest += toPlayer.normalized * coreCableEndClearance;
                        b = closest;
                    }

                    coreCable.enabled = true;
                    coreCable.startWidth = coreCableWidth;
                    coreCable.endWidth = coreCableWidth;
                    SetSaggingLine(coreCable, a, b, coreCableSegments, coreCableSag, coreCableGroundLayers, coreCableGroundClearance);
                }
                else
                {
                    coreCable.enabled = false;
                }
            }
        }
        else
        {
            coreCable.enabled = false;
        }
    }

    void SetSaggingLine(LineRenderer lr, Vector3 start, Vector3 end, int segments, float sag, LayerMask groundLayers, float groundClearance)
    {
        int seg = Mathf.Max(1, segments);
        lr.positionCount = seg + 1;

        Vector3 delta = end - start;
        float distance = delta.magnitude;
        float sagScaled = sag * Mathf.Clamp(distance, 0f, 50f) / 10f;

        for (int i = 0; i <= seg; i++)
        {
            float t = (float)i / seg;
            Vector3 p = Vector3.Lerp(start, end, t);
            float curve = 4f * t * (1f - t);
            p += Vector3.down * (sagScaled * curve);

            if (groundLayers.value != 0)
            {
                Vector3 origin = p + Vector3.up * 10f;
                if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 50f, groundLayers, QueryTriggerInteraction.Ignore))
                {
                    float minY = hit.point.y + Mathf.Max(0f, groundClearance);
                    if (p.y < minY)
                        p.y = minY;
                }
            }
            lr.SetPosition(i, p);
        }
    }
}
