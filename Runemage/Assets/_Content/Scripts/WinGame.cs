using System.Collections;
using System.Collections.Generic;
using _Content.Scripts.Data.Containers.GlobalSignal;
using Data.Enums;
using Data.Interfaces;
using Singletons;
using UnityEngine;

public class WinGame : MonoBehaviour, IReceiveGlobalSignal
{
    // Start is called before the first frame update
    void Start()
    {
        GlobalMediator.Instance.Subscribe(this);
    }

    // Update is called once per frame
    void Update()
    {
        
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
