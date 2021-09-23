using System;
using _Content.Scripts.Data.Containers.GlobalSignal;
using Data.Enums;
using Data.Interfaces;
using UnityEngine;

namespace Singletons
{
    public class GameManager : MonoBehaviour, IReceiveGlobalSignal, ISendGlobalSignal

    {
        private static GameManager instance;

        public static GameManager Instance
        {
            get { return instance; }
        }

        public bool gamePaused;
        public bool gestureTrainingMode;
        public bool usePcInput;

        public GlobalEvent currentGameState;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            GlobalMediator.Instance.Subscribe(this);
        }

        public void ChangeGameState(GlobalEvent newGameState)
        {
            if (currentGameState == newGameState)
            {
                return;
            }

            switch (newGameState)
            {
                case GlobalEvent.WIN_GAME:
                    
                    break;
                
                case GlobalEvent.LOST_GAME:

                    break;
                
                case GlobalEvent.PAUSE_GAME:

                    break;
                
                case GlobalEvent.UNPAUSE_GAME:

                    break;
            }
        }
        
        public void ReceiveGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
        {
            switch (eventState)
            {
                case GlobalEvent.WIN_GAME:

                    break;

                case GlobalEvent.LOST_GAME:

                    break;
                
                case GlobalEvent.PAUSE_GAME:

                    gamePaused = true;
                    break;
                
                case GlobalEvent.UNPAUSE_GAME:

                    gamePaused = false;
                    break;
            }
        }

        public void SendGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
        {
            GlobalMediator.Instance.ReceiveGlobal(eventState, globalSignalData);
        }
    }
}