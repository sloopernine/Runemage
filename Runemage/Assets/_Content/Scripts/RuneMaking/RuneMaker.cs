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
			case GlobalEvent.CREATE_SPELL_ORIGIN: 
				if(globalSignalData is RuneData runeData)
				{
					switch(runeData.result.GestureClass)
					{
						case "Circle":
							Debug.Log("RuneMaker is going to instantiate a spellCastOrigin with Fireball");
							Debug.Log("RuneMaker is going to instantiate with spell-type CIRCLE");

							Instantiate(spellCastOrigin);
							spellCastOrigin.gameObject.transform.position = runeData.position;
							spellCastOrigin.gameObject.transform.eulerAngles = runeData.angle;
							spellCastOrigin.gameObject.transform.localScale = runeData.scale;
							spellCastOrigin.currentSpell = Spell.Fireball;
							break;
						case "Fireball":
							Debug.Log("RuneMaker is going to instantiate a spellCastOrigin with Fireball");
							Debug.Log("RuneMaker is going to instantiate with spell-type FIREBALL");

							Instantiate(spellCastOrigin);
							spellCastOrigin.gameObject.transform.position = runeData.position;
							spellCastOrigin.gameObject.transform.eulerAngles = runeData.angle;
							spellCastOrigin.gameObject.transform.localScale = runeData.scale;
							spellCastOrigin.currentSpell = Spell.Fireball;
							break;
						case "Ice":
							Debug.Log("RuneMaker is going to instantiate a spellCastOrigin with Fireball.");
							Debug.Log("RuneMaker is going to instantiate with spell-type ICE");

							Instantiate(spellCastOrigin);
							spellCastOrigin.gameObject.transform.position = runeData.position;
							spellCastOrigin.gameObject.transform.eulerAngles = runeData.angle;
							spellCastOrigin.gameObject.transform.localScale = runeData.scale;
							spellCastOrigin.currentSpell = Spell.Fireball;
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
