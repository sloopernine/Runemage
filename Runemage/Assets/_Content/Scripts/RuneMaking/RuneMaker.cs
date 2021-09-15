using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Interfaces;
using Singletons;
using Data.Enums;
using _Content.Scripts.Data.Containers.GlobalSignal;

public class RuneMaker : MonoBehaviour, IReceiveGlobalSignal
{
	public SpellCastOrigin spellCastOrigin;

	void Start()
	{
		GlobalMediator.Instance.Subscribe(this);
	}
	
	private void OnDestroy()
	{
		GlobalMediator.Instance.UnSubscribe(this);
	}

	public void ReceiveGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
	{
		switch (eventState) {
			case GlobalEvent.CREATE_SPELL: 
				if(globalSignalData is RuneData runeData)
				{
					switch(runeData.result.GestureClass)
					{
						case "Circle":
							Instantiate(spellCastOrigin);
							spellCastOrigin.gameObject.transform.position = runeData.position;

							break;
						case "Fireball":

							break;
						case "Ice":

							break;
						default:
							Debug.Log("The switch lacks a case that compares to the gestureclass!");
							break;
					}
				}
				break;

		}
	}

}
