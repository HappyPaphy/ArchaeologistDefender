using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnList
{
    [SerializeField] public List<float> enemySpawnCooldownList;
    [SerializeField] public List<int> enemySpawnPositionIndexList;
    [SerializeField] public List<int> enemySpawnIndexList;
    [SerializeField] public int enemySpawnCountList;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform[] enemySpawn;
    [SerializeField] private GameObject[] enemyPrefabs;

    public enum WaveState { Wave_1, Wave_2, Wave_3, Wave_4, Wave_5 };
    [SerializeField] private WaveState currentWaveState;

    [SerializeField] private float nextWave;
    [SerializeField] private float waveBreakDuration;

    [SerializeField] private List<EnemySpawnList> enemySpawnLists;
    [SerializeField] private bool isSpawning = false;
    [SerializeField] private bool isWaveFinished = true;

    public static EnemySpawner instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        StartCoroutine(EnemyWave());
    }

    private void Update()
    {
        /*if (!isSpawning && isWaveFinished)
        {
            StartCoroutine(EnemyWave());
        }*/
    }

    private IEnumerator EnemyWave()
    {
        isSpawning = true;

        switch (currentWaveState)
        {
            case WaveState.Wave_1:
                yield return StartCoroutine(HandleWave(enemySpawnLists[0], 2f));
                currentWaveState = WaveState.Wave_2;
                break;

            case WaveState.Wave_2:
                yield return StartCoroutine(HandleWave(enemySpawnLists[1], 5f));
                currentWaveState = WaveState.Wave_3;
                break;

            case WaveState.Wave_3:
                // Add more waves if needed
                break;
        }

        isSpawning = false;
    }

    private IEnumerator HandleWave(EnemySpawnList spawnList, float initialWait)
    {
        yield return new WaitForSeconds(initialWait);

        for (int i = 0; i < spawnList.enemySpawnCountList; i++)
        {
            yield return StartCoroutine(SpawnEnemy(
                spawnList.enemySpawnIndexList[i],
                spawnList.enemySpawnPositionIndexList[i],
                spawnList.enemySpawnCooldownList[i]));
        }

        isWaveFinished = true;  // Mark the wave as finished once all enemies are spawned
    }

    IEnumerator WaitTime(float value)
    {
        yield return new WaitForSeconds(value);
    }

    IEnumerator SpawnEnemy(int enemyIndex, int spawnIndex, float spawnCooldown)
    {
        yield return new WaitForSeconds(spawnCooldown);
        GameObject enemy = Instantiate(enemyPrefabs[enemyIndex]);
        enemy.transform.position = enemySpawn[spawnIndex].position;
        isSpawning = false;
    }
}
