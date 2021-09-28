using System.Collections.Generic;
using UnityEngine;
using Data.Interfaces;
using Data.Enums;
using _Content.Scripts.Data.Containers.GlobalSignal;
using Singletons;

public class EnemyRoundObserver : MonoBehaviour, IReceiveGlobalSignal, ISendGlobalSignal
{
    private RoundHandler roundHandler;
    private int deadEnemies;

    private void Start()
    {
        GlobalMediator.Instance.Subscribe(this);

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
        deadEnemies++;
        if (deadEnemies == roundHandler.getRoundTotalEnemies)
        {
            roundHandler.UpdateRound();
            deadEnemies = 0;
            
            SendGlobal(GlobalEvent.PAUSED_GAMESTATE);
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

    public void SendGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
    {
        GlobalMediator.Instance.ReceiveGlobal(eventState, globalSignalData);
    }
}
