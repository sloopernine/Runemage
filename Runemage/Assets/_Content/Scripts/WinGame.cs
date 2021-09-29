using System.Collections;
using System.Collections.Generic;
using _Content.Scripts.Data.Containers.GlobalSignal;
using Data.Enums;
using Data.Interfaces;
using Singletons;
using UnityEditor;
using UnityEngine;

public class WinGame : MonoBehaviour, IReceiveGlobalSignal
{
    public GameObject mistObjects;
    public bool winGame;

    private Vector3 startPosition;
    private Vector3 startSize;
    
    void Start()
    {
        GlobalMediator.Instance.Subscribe(this);

        startPosition = transform.position;
        startSize = transform.localScale;
    }

    // Update is called once per frame
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
    }

    public void ReceiveGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
    {
        switch (eventState)
        {
            case GlobalEvent.WIN_GAMESTATE:
                
                break;
        }
    }
}
