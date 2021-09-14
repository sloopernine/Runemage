using System.Collections.Generic;
using UnityEngine;
using Data.Interfaces;
using Data.Enums;
using _Content.Scripts.Data.Containers.GlobalSignal;
using Singletons;

public class EnemyRoundObserver : MonoBehaviour, IReceiveGlobalSignal
{
    RoundHandler roundHandler;

    private void Start()
    {
        GlobalMediator.INSTANCE.Subscribe(this);

        roundHandler = FindObjectOfType<RoundHandler>();
        if (roundHandler == null)
        {
            Debug.LogError("Scene is missing Round Handler and the Enemy Round Observer wont work");
            return;
        }
    }

    //This is still WIP
    private void Observe()
    {
        Debug.Log("Observer was observing");
        List<Enemy> allEnemies = EnemyPool.Instance.pooledEnemies;
        int deadEnemies = 0;

        for (int i = 0; i < allEnemies.Count; i++)
        {
            if (allEnemies[i].gameObject.activeInHierarchy == false)
            {
                deadEnemies++;
                Debug.Log(deadEnemies);
                if (deadEnemies == allEnemies.Count)
                {
                    roundHandler.UpdateCurrentRound();
                }
            }
        }       
    }

    public void ReceiveGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
    {
        switch(eventState)
        {
            case GlobalEvent.OBJECT_INACTIVE:
            {
                if (globalSignalData is BasicData data)
                {
                        if (data.stringValue == "Enemy")
                        {
                            Observe();
                        }
                }
                break;
            }   
                    
            default:
            {
                break;
            }            
        }                      
    }
}
