using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerHandler : MonoBehaviour
{
    public EnemyWave testWave;

    private EnemySpawner[] enemySpawners;
    private List<EnemySpawner> currentSpawners;

    public bool startWave;

    private void Start()
    {
        enemySpawners = FindObjectsOfType<EnemySpawner>();
        currentSpawners = new List<EnemySpawner>();
    }

    public void Update()
    {
        if (startWave)
        {
            CreateWave(testWave);
        }
    }

    public void CreateWave(EnemyWave enemyWave)
    {
        SetupSpawners(enemyWave.numberOfSpawners);
        SetupWave(enemyWave.enemy, enemyWave.numberOfEnemies);
        SpawnWave();
        startWave = false;
    }

    public void SetupSpawners(int numberOfSpawners)
    {
        if (numberOfSpawners > enemySpawners.Length)
        {
            Debug.LogWarning("SetupWave() CANCELED: The input of spawners are more then the spawners in the scene");
            return;
        }

        for (int i = 0; i < numberOfSpawners; i++)
        {
            //Random up spawners later
            currentSpawners.Add(enemySpawners[i]);
        }
    }

    public void SetupWave(Enemy enemy, int spawnNumber)
    {      
        foreach(EnemySpawner spawner in currentSpawners)
        {
            spawner.AddNewEnemies(enemy, spawnNumber);
        }
    }

    public void SpawnWave()
    {
        foreach (EnemySpawner spawner in currentSpawners)
        {
            spawner.spawnEnemies = true;
        }
        currentSpawners.Clear();
    }
}