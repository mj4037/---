using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] List<Transform> positions;
    [SerializeField] List<Unit> unitPrefabs;
    [SerializeField] List<Unit> bossPrefabs;

    [SerializeField] float baseCooldown = 0.9f;
    [SerializeField] float createCooldown = 0.9f;
    [SerializeField] int waveCreateCount = 10;
    [SerializeField] int maxCreateCount = 30;

    [SerializeField] public Dictionary<Unit, List<Unit>> pools = new Dictionary<Unit, List<Unit>>();
    [SerializeField] public List<Unit> unitList = new List<Unit>();

    [SerializeField] private int aliveCount = 0;
    private int random = 0;

    private int bossCount = 0;

    Coroutine spawnCoroutine;

    private void Awake()
    {
        // GameManager.Instance may not be initialized yet depending on script execution order.
    }

    void Start()
    {
        UnitListFull();
    }

    private void OnEnable()
    {
        State.Subscribe(Condition.BATTLE, StartSpawn);
        State.Subscribe(Condition.FINISH, StopSpawn);
    }

    private void OnDisable()
    {
        State.Unsubscribe(Condition.BATTLE, StartSpawn);
        State.Unsubscribe(Condition.FINISH, StopSpawn);
    }

    void StartSpawn()
    {
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(CreateRoutine());
        }
    }

    void StopSpawn()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    void UnitListFull()
    {
        if (GameManager.Instance == null || GameManager.Instance.Wave == null)
            return;

        if (positions == null || positions.Count == 0)
            return;

        if (unitPrefabs == null || unitPrefabs.Count == 0)
            return;

        aliveCount = waveCreateCount;

        while(unitList.Count < waveCreateCount)
        {
            if (GameManager.Instance.Wave.wave >= 6)
            {
                random = Random.Range(0, 7);

                if (random == 6)
                {
                    GetPool(unitPrefabs[2]);

                    continue;
                }
            }

            if (GameManager.Instance.Wave.wave >= 3)
            {
                random = Random.Range(0, 3);

                if (random == 2)
                {
                    GetPool(unitPrefabs[1]);

                    continue;
                }
            }

            GetPool(unitPrefabs[0]);
            
        }
    }

    void GetPool(Unit prefab)
    {
        if (pools.ContainsKey(prefab) == false)
        {
            pools[prefab] = new List<Unit>();
        }

        Unit unit = null;

        if (pools[prefab].Count == 0)
        {
            unit = Instantiate(prefab, positions[Random.Range(0, positions.Count)].position, Quaternion.identity, transform);
        }
        else
        {
            unit = pools[prefab][pools[prefab].Count - 1];
            pools[prefab].RemoveAt(pools[prefab].Count - 1);
        }

        unit.gameObject.SetActive(false);

        unit.ParentPrefab = prefab;

        unitList.Add(unit);
    }

    public void Release(Unit prefab, Unit unit)
    {
        aliveCount--;

        unit.transform.position = positions[Random.Range(0, positions.Count - 1)].position;

        unit.gameObject.SetActive(false);

        if (pools.ContainsKey(prefab) == false)
        {
            pools.Add(prefab, new List<Unit>());
        }

        pools[prefab].Add(unit);

        Debug.Log(aliveCount);
    }

    IEnumerator CreateRoutine()
    {
        while (true)
        {
            for (int i = 0; i < waveCreateCount; i++)
            {
                unitList[0].gameObject.SetActive(true);

                unitList.RemoveAt(0);

                Debug.Log("Create");

                yield return CoroutineCache.WaitForSeconds(createCooldown);
            }

            yield return new WaitUntil(() => aliveCount == 0);

            GameManager.Instance.Wave.WaveUp();

            if (GameManager.Instance.Wave.BossWave())
            {
                GetPool(bossPrefabs[bossCount++ % bossPrefabs.Count]);

                if (waveCreateCount < maxCreateCount)
                {
                    waveCreateCount = Mathf.Min(maxCreateCount, waveCreateCount + 10);
                }
            }

            UnitListFull();

            Debug.Log(waveCreateCount);

            createCooldown -= GameManager.Instance.Wave.wave * 0.03f;
        }
    }
}
