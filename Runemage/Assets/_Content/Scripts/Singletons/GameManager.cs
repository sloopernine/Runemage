using System;
using _Content.Scripts.Data.Containers.GlobalSignal;
using Data.Enums;
using Data.Interfaces;
using UnityEngine;

namespace Singletons
{
	public class GameManager : MonoBehaviour, IReceiveGlobalSignal {
		private static GameManager instance;

		public static GameManager Instance {
			get { return instance; }
		}

		private GlobalEvent currentGameState;
		public GlobalEvent CurrentGameState {
			get { return currentGameState; }
		}

		public bool gestureTrainingMode;
		public bool usePcInput;

		private void Awake() {
			if (instance == null) {
				instance = this;
			} else {
				Destroy(gameObject);
			}
		}

		private void Start() {
			GlobalMediator.Instance.Subscribe(this);
			
			currentGameState = GlobalEvent.PAUSED_GAMESTATE;

			if (usePcInput) {
				//Cursor.lockState = CursorLockMode.Locked;
			}
		}

		private void Update() {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				Application.Quit();
			}
		}

		public void ReceiveGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null) {
			switch (eventState) {
				case GlobalEvent.WIN_GAMESTATE:
					
					currentGameState = GlobalEvent.WIN_GAMESTATE;
					break;

				case GlobalEvent.LOST_GAMESTATE:

					currentGameState = GlobalEvent.LOST_GAMESTATE;
					break;

				case GlobalEvent.PAUSED_GAMESTATE:

					currentGameState = GlobalEvent.PAUSED_GAMESTATE;
					break;

				case GlobalEvent.PLAY_GAMESTATE:

					currentGameState = GlobalEvent.PLAY_GAMESTATE;
					break;
			}
		}
	}
}