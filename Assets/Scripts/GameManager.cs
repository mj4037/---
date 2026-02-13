using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public WaveManager wave;
    [SerializeField] public SpawnManager spawn;
    [SerializeField] public Core core;
    [SerializeField] public Player player;

    public WaveManager Wave => wave;
    public SpawnManager Spawn => spawn;
    public Core Core => core;
    public Player Player => player;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ResolveRefs();
    }

    void ResolveRefs()
    {
        if (wave == null) wave = FindFirstObjectByType<WaveManager>();
        if (spawn == null) spawn = FindFirstObjectByType<SpawnManager>();
        if (core == null) core = FindFirstObjectByType<Core>();
        if (player == null) player = FindFirstObjectByType<Player>();
    }
}