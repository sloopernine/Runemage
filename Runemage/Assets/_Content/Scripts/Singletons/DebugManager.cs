using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Interfaces;
using Data.Enums;
using _Content.Scripts.Data.Containers.GlobalSignal;
using Singletons;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class DebugManager : MonoBehaviour, ISendGlobalSignal, IReceiveGlobalSignal
{

    [SerializeField] bool isDebugMode;
    public bool IsDebugMode { get => isDebugMode;}
    private Text debugUi;
    [SerializeField] float debugMessageTime = 5f;


    private void Start()
    {
        debugUi = GetComponentInChildren<Text>();
        if (isDebugMode)
        {
            StartCoroutine(ShowDebugMessage("DebugMode_On", debugMessageTime));
        }
        else
        {
            StartCoroutine(ShowDebugMessage("DebugMode_Off", debugMessageTime));
        }
    }

    private void OnEnable()
    {
        GlobalMediator.Instance.Subscribe(this);
    }

    private void OnDisable()
    {
        GlobalMediator.Instance.UnSubscribe(this);

    }

    private void Update()
    {
        if (!isDebugMode)
        {
            return;
        }
        // Suggestion to add shield health indicator on debug hud

    }

    public void ReceiveGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
    {

        switch (eventState)
        {
            case GlobalEvent.DEBUG_ON:
                SetDebugMode(true);
                break;
            case GlobalEvent.DEBUG_OFF:
                SetDebugMode(false);
                break;
        }

        if (!isDebugMode)
        {
            return;
        }

    }

    private void SetDebugMode(bool isOn)
    {
        StopAllCoroutines();
        isDebugMode = isOn;
        if (isOn)
        {
            StartCoroutine(ShowDebugMessage("DebugMode_On", debugMessageTime));
        }
        else
        {
            StartCoroutine(ShowDebugMessage("DebugMode_Off", debugMessageTime));
        }

    }

    IEnumerator ShowDebugMessage(string message, float timeVisible)
    {
        debugUi.text = message;
        yield return new WaitForSeconds(timeVisible);
        debugUi.text = "";
    }

    public void SendGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
    {
        GlobalMediator.Instance.ReceiveGlobal(eventState, globalSignalData);
        StartCoroutine(ShowDebugMessage(eventState.ToString(), debugMessageTime));
    }

    public void KillAllEnemies()
    {
        SendGlobal(GlobalEvent.ENEMY_DESTROY_ALL);

    }

    public void Invulnerability()
    {
        SendGlobal(GlobalEvent.SHIELD_INVULNERABLE_ON);
    }

    public void Invulnerability_Off()
    {
        SendGlobal(GlobalEvent.SHIELD_INVULNERABLE_OFF);
    }

    public void ForceNextRound()
    {
        SendGlobal(GlobalEvent.ENEMY_DESTROY_ALL);
        SendGlobal(GlobalEvent.SET_NEXT_ROUND);
    }

    public void ForceFirstRound()
    {
        SendGlobal(GlobalEvent.ENEMY_DESTROY_ALL);
        BasicData roundNumber = new BasicData(0);
        SendGlobal(GlobalEvent.SET_ROUND, roundNumber);
    }

    public void DestroyAllSpells()
    {
        SendGlobal(GlobalEvent.SPELLS_DESTROY_ALL);
    }

	/// <summary>
	/// Destroys all runeclouds
	/// </summary>
	public void DestroyAllRuneClouds()
	{
		SendGlobal(GlobalEvent.RUNECLOUD_DESTROYALL);
	}

    public void ResetShield()
    {
        SendGlobal(GlobalEvent.SHIELD_RESET);

    }

    public void SetGameStatePaused()
    {
        SendGlobal(GlobalEvent.PAUSED_GAMESTATE);

    }

    public void SetGameStatePlay()
    {
        SendGlobal(GlobalEvent.PLAY_GAMESTATE);

    }

    public void SetGameStateWin()
    {
        SendGlobal(GlobalEvent.WIN_GAMESTATE);
    }
    
    public void SetGameStateLose()
    {
        SendGlobal(GlobalEvent.LOST_GAMESTATE);
    }
}
