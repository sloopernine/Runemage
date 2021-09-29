using System.Collections;
using System.Collections.Generic;
using _Content.Scripts.Data.Containers.GlobalSignal;
using Data.Enums;
using Data.Interfaces;
using Singletons;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGame : MonoBehaviour, IReceiveGlobalSignal, ISendGlobalSignal
{
	public Renderer planetRenderer1;
	public Renderer planetRenderer2;
	public float planetEmissiveIntensity;
    public GameObject mistObjects;
    public bool winGame;
    public bool loseGame;

    private Vector3 startPosition;
    private Vector3 startSize;
    private static readonly int FrenselPower = Shader.PropertyToID("_FrenselPower");

    void Start()
    {
        GlobalMediator.Instance.Subscribe(this);

        startPosition = transform.position;
        startSize = transform.localScale;

        planetEmissiveIntensity = 15f;
    }

    void Update()
    {
        if (winGame)
        {
            if (mistObjects.transform.position.y > -32f)
            {
                mistObjects.transform.Translate(0f,-0.05f,0f);
            }

            if (mistObjects.transform.localScale.x < 10)
            {
                mistObjects.transform.localScale += new Vector3(0.8f,0,0.8f) * Time.deltaTime;
            }
        }

        if (loseGame)
        {
	        if (mistObjects.transform.localScale.x > 0.18f)
	        {
		        mistObjects.transform.localScale += new Vector3(-0.2f,0,-0.2f) * Time.deltaTime;
	        }

	        if (planetEmissiveIntensity > -6f)
	        {
		        planetEmissiveIntensity -= 6.5f * Time.deltaTime;
		        planetRenderer1.material.SetFloat(FrenselPower, planetEmissiveIntensity);
		        planetRenderer2.material.SetFloat(FrenselPower, planetEmissiveIntensity);
	        }
	        else if (planetEmissiveIntensity <= -6f)
	        {
		        SceneManager.LoadScene(0);
	        }
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

				//OnGameLost();
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

	public void SendGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null) 
	{
		GlobalMediator.Instance.ReceiveGlobal(eventState, globalSignalData);
	}
}