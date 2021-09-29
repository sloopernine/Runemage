using System.Collections;
using System.Collections.Generic;
using _Content.Scripts.Data.Containers.GlobalSignal;
using Data.Enums;
using Data.Interfaces;
using Singletons;
using UnityEditor;
using UnityEngine;

public class WinGame : MonoBehaviour, IReceiveGlobalSignal, ISendGlobalSignal
{
    public GameObject mistObjects;
    public bool winGame;
    public bool loseGame;

    private Vector3 startPosition;
    private Vector3 startSize;
    
    void Start()
    {
        GlobalMediator.Instance.Subscribe(this);

        startPosition = transform.position;
        startSize = transform.localScale;
    }

    void Update()
    {
        if (winGame)
        {
            if (mistObjects.transform.position.y > -32f)
            {
                mistObjects.transform.Translate(0f,-0.05f,0f);
            }

            if (mistObjects.transform.localScale.y < 10)
            {
                mistObjects.transform.localScale += new Vector3(0.8f,0,0.8f) * Time.deltaTime;
            }
        }

        if (loseGame)
        {
	        OnGameLost();
	        loseGame = false;
        }
    }

    public void ReceiveGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
    {
        switch (eventState)
        {
            case GlobalEvent.WIN_GAMESTATE:

	            winGame = true;
                break;
			case GlobalEvent.LOST_GAMESTATE:

				loseGame = true;
				break;
        }
    }
    
	private void OnGameLost()
	{
		Debug.Log("Game is preparing to lose");

		SendGlobal(GlobalEvent.ENEMY_DESTROY_ALL);
		BasicData roundNumber = new BasicData(0);
	
		SendGlobal(GlobalEvent.SET_ROUND, roundNumber);
		
		SendGlobal(GlobalEvent.SPELLS_DESTROY_ALL);
		
		SendGlobal(GlobalEvent.SHIELD_RESET);

		Debug.Log("Game should now let planets shine too bright.");
	}

	public void SendGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null) {
		GlobalMediator.Instance.ReceiveGlobal(eventState, globalSignalData);
	}
}