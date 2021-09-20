using _Content.Scripts.Data.Containers.GlobalSignal;
using Data.Enums;
using Data.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singletons;

public class EnemyPool : MonoBehaviour , IReceiveGlobalSignal
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

    private void OnEnable()
    {
        GlobalMediator.Instance.Subscribe(this);
    }

    private void OnDisable()
    {
        GlobalMediator.Instance.UnSubscribe(this);

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

    public void ReceiveGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
    {
        switch (eventState)
        {
            case GlobalEvent.ENEMY_DESTROY_ALL:
                KillAllEnemies();
                break;

        }
    }

    private void KillAllEnemies()
    {
        foreach (Enemy enemy in pooledEnemies)
        {
            
            if (enemy.gameObject.activeInHierarchy)
            {
                enemy.Die();
            }
        }
    }

}
