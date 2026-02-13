using UnityEngine;

public class TurretCoreCable : MonoBehaviour
{
    static TurretCoreCable active;

    [SerializeField] bool connected;
    [SerializeField, Min(0f)] float cableWidth = 0.12f;
    [SerializeField, Min(1)] int cableSegments = 12;
    [SerializeField, Min(0f)] float cableSag = 1.2f;
    [SerializeField] LayerMask cableGroundLayers = ~0;
    [SerializeField, Min(0f)] float cableGroundClearance = 0.05f;
    [SerializeField] Color cableColor = new Color(0.2f, 0.8f, 1f, 1f);
    [SerializeField] Vector3 turretOffset = new Vector3(0f, 1.2f, 0f);
    [SerializeField] Vector3 coreOffset = new Vector3(0f, 1.2f, 0f);
    [SerializeField, Min(0f)] float coreEndClearance = 0.05f;

    [SerializeField] Vector3 indicatorOffset = new Vector3(0f, 2.2f, 0f);
    [SerializeField, Min(0f)] float indicatorSize = 0.25f;
    [SerializeField] Color indicatorColor = new Color(1f, 0.9f, 0.1f, 1f);

    LineRenderer lr;
    Transform core;
    Transform indicator;
    TextMesh indicatorText;

    void SetSaggingLine(LineRenderer line, Vector3 start, Vector3 end, int segments, float sag, LayerMask groundLayers, float groundClearance)
    {
        int seg = Mathf.Max(1, segments);
        line.positionCount = seg + 1;

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
            line.SetPosition(i, p);
        }
    }

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        if (lr == null)
            lr = gameObject.AddComponent<LineRenderer>();

        lr.enabled = false;
        lr.positionCount = Mathf.Max(2, cableSegments + 1);
        lr.useWorldSpace = true;
        lr.startWidth = cableWidth;
        lr.endWidth = cableWidth;
        lr.startColor = cableColor;
        lr.endColor = cableColor;

        if (lr.material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");

            if (shader != null)
            {
                lr.material = new Material(shader);
                if (lr.material.HasProperty("_BaseColor"))
                    lr.material.SetColor("_BaseColor", cableColor);
                else if (lr.material.HasProperty("_Color"))
                    lr.material.SetColor("_Color", cableColor);
            }
        }

        GameObject go = new GameObject("CableIndicator");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = indicatorOffset;
        indicator = go.transform;
        indicatorText = go.AddComponent<TextMesh>();
        indicatorText.text = "!";
        indicatorText.color = indicatorColor;
        indicatorText.fontSize = 64;
        indicatorText.characterSize = indicatorSize;
        indicatorText.anchor = TextAnchor.MiddleCenter;
        indicatorText.alignment = TextAlignment.Center;
        go.SetActive(false);
    }

    void OnDisable()
    {
        if (active == this)
            active = null;
    }

    public static void DeactivateActive()
    {
        if (active != null)
            active.Disconnect();
    }

    public void ConnectToCore(Transform coreTransform)
    {
        if (active != null && active != this)
            active.Disconnect();

        core = coreTransform;
        connected = core != null;
        if (lr != null)
            lr.enabled = connected;

        if (indicator != null)
            indicator.gameObject.SetActive(connected);

        active = connected ? this : null;
    }

    public void Disconnect()
    {
        connected = false;
        core = null;
        if (lr != null)
            lr.enabled = false;

        if (indicator != null)
            indicator.gameObject.SetActive(false);

        if (active == this)
            active = null;
    }

    void Update()
    {
        if (!connected || lr == null)
            return;

        if (indicator != null)
        {
            indicator.localPosition = indicatorOffset;
            if (Camera.main != null)
            {
                Vector3 camPos = Camera.main.transform.position;
                Vector3 dir = indicator.position - camPos;
                if (dir.sqrMagnitude > 0.0001f)
                    indicator.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
            }
        }

        if (core == null)
        {
            Core c = GameManager.Instance != null ? GameManager.Instance.Core : null;
            if (c != null) core = c.transform;
        }

        if (core == null)
        {
            lr.enabled = false;
            connected = false;
            return;
        }

        Vector3 a = transform.position + turretOffset;
        Vector3 b = core.position + coreOffset;
        Collider coreCol = core.GetComponent<Collider>();
        if (coreCol != null)
        {
            Vector3 closest = coreCol.ClosestPoint(a);
            Vector3 toTurret = a - closest;
            if (toTurret.sqrMagnitude > 0.0001f)
                closest += toTurret.normalized * coreEndClearance;
            b = closest;
        }

        lr.enabled = true;
        lr.startWidth = cableWidth;
        lr.endWidth = cableWidth;
        SetSaggingLine(lr, a, b, cableSegments, cableSag, cableGroundLayers, cableGroundClearance);
    }
}
