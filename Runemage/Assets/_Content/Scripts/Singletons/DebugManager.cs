using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Interfaces;
using Data.Enums;
using _Content.Scripts.Data.Containers.GlobalSignal;
using Singletons;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour, ISendGlobalSignal, IReceiveGlobalSignal
{

    [SerializeField] bool isDebugMode;
    private Text debugUi;
    [SerializeField] float debugMessageTime = 5f;

    private void Start()
    {
        debugUi = GetComponentInChildren<Text>();
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

        if (Input.GetKeyDown(KeyCode.P))
        {
            //SetDebugMode(!isDebugMode);
            SendGlobal(GlobalEvent.ENEMY_DESTROY_ALL);
        }
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
        switch (eventState)
        {
            case GlobalEvent.WIN_GAME:
                break;
            case GlobalEvent.PAUSE_GAME:
                break;
            case GlobalEvent.UNPAUSE_GAME:
                break;

            case GlobalEvent.SHOW_FPS:
                break;
            case GlobalEvent.HIDE_FPS:
                break;

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
    }
}
