using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] Enemy enemyPrefab;
    [SerializeField] int numberToSpawn;

    [Header("Spawner")]
    [Tooltip("Start Line should be in the right side of the spawner")]
    public Transform startLine;

    [Tooltip("End Line should be in the left side of the spawner")]
    public Transform endLine;

    public bool spawnEnemies;

    [Header("Gizmos")]
    public Color gizmosColor = Color.magenta;
    public bool showIcon;
    public string spawnerIcon;

    private void Update()
    {
        if (spawnEnemies)
        {
            SpawnEnemies(enemyPrefab, numberToSpawn);
        }
    }

    private void SpawnEnemies(Enemy enemyType, int spawnNumber)
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            Vector3 spawnPostion = RandomPositionOnLine();
            Quaternion spawnRotation = CalculateRotation();
            Instantiate(enemyType.gameObject, spawnPostion, spawnRotation);
        }

        spawnEnemies = false;
    }

    private Vector3 RandomPositionOnLine()
    {
        float xRange = endLine.position.x - startLine.position.x;
        float yRange = endLine.position.y - startLine.position.y;
        float zRange = endLine.position.z - startLine.position.z;

        Vector3 spawnPosition = new Vector3(startLine.position.x + (xRange * Random.value),
                                            startLine.position.y + (yRange * Random.value),
                                            startLine.position.z + (zRange * Random.value));

        return spawnPosition;
    }

    private Quaternion CalculateRotation()
    {
        Quaternion spawnRotation = Quaternion.LookRotation(startLine.position - endLine.position);
        spawnRotation.eulerAngles += new Vector3(0, 90f, 0);

        return spawnRotation;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        Gizmos.DrawLine(startLine.position, endLine.position);

        if (showIcon)
        {
            Gizmos.DrawIcon(startLine.position, spawnerIcon);
            Gizmos.DrawIcon(endLine.position, spawnerIcon);
        }
    }

    public void AddNewEnemies(Enemy newEnemy, int spawnNumber)
    {
        enemyPrefab = newEnemy;
        numberToSpawn = spawnNumber;
    }
}