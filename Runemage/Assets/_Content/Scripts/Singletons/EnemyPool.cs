using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    private static EnemyPool instance;
    public static EnemyPool Instance { get { return instance; } }
    public List<Enemy> pooledEnemies;
    public Enemy enemyToPool;
    public int amoutToPool;

	private void Awake()
	{
        instance = this;
	}

    private void Start()
    {
        pooledEnemies = new List<Enemy>();
        Enemy tempEnemy;
        for (int i = 0; i < amoutToPool; i++)
        {
            tempEnemy = Instantiate(enemyToPool);
            tempEnemy.gameObject.SetActive(false);
            pooledEnemies.Add(tempEnemy);
        }      
    }

    public Enemy GetPooledEnemy()
    {
        for (int i = 0; i < amoutToPool; i++)
        {
            if(!pooledEnemies[i].gameObject.activeInHierarchy)
            {
                return pooledEnemies[i];
            }
        }       
        return null;
    }

}
