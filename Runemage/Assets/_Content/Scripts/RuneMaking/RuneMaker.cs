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
					switch(runeData.result.spell)
					{
						case Spell.Fireball:
							Instantiate(spellCastOrigin);
							spellCastOrigin.gameObject.transform.position = runeData.position;
							spellCastOrigin.gameObject.transform.eulerAngles = runeData.angle;
							spellCastOrigin.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f); //runeData.scale;
							spellCastOrigin.currentSpell = Spell.Fireball;
							break;
						case Spell.Ice:
							Instantiate(spellCastOrigin);
							spellCastOrigin.gameObject.transform.position = runeData.position;
							spellCastOrigin.gameObject.transform.eulerAngles = runeData.angle;
							spellCastOrigin.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f); //runeData.scale;
							spellCastOrigin.currentSpell = Spell.Ice;
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
