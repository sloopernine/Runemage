using UnityEngine;
using Data.Interfaces;
using Singletons;
using Data.Enums;
using _Content.Scripts.Data.Containers.GlobalSignal;

public class RuneMaker : MonoBehaviour, IReceiveGlobalSignal
{
	[SerializeField] GameObject iceSpellPrefab;
	[SerializeField] GameObject fireSpellPrefab;

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
				GameObject tempSpell;
					
					switch(runeData.result.spell)
					{
						case Spell.Fireball:
							tempSpell = Instantiate(fireSpellPrefab, runeData.position, Quaternion.identity);
							break;
						case Spell.Ice:
							tempSpell = Instantiate(iceSpellPrefab, runeData.position, Quaternion.identity);
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
