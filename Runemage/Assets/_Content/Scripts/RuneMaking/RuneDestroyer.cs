using System.Collections.Generic;
using UnityEngine;
using Data.Interfaces;
using Data.Enums;
using _Content.Scripts.Data.Containers.GlobalSignal;
using Singletons;

/// <summary>
/// On creation a runecloud adds itself to a list in this script.
/// Periodically this script goes through the list and checks each runecloud.
/// if it still exists && has too few points && has lived for too long
/// It tells it to destroy itself.
/// </summary>
public class RuneDestroyer : MonoBehaviour, IReceiveGlobalSignal, ISendGlobalSignal
{
	[Header("Update time")]
	[SerializeField] [Min(0.1f)] float checkIntervall;
	private float timeSinceCheck;

	[Header("Criteria for judging runeclouds")]
	[SerializeField] [Min(0)] int minimumPoints;
	[SerializeField] private float minRuneCloudAge;

	public static RuneDestroyer Instance;
	
	private List<RuneCloud> runeClouds;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		} 
		else
		{
			Destroy(gameObject);
		}
	}

	void Start()
	{
		runeClouds = new List<RuneCloud>();
	}

	void Update()
	{
		//protects in case runeCloud is empty
		if(runeClouds.Count <= 0 || runeClouds == null)
		{
			timeSinceCheck += Time.deltaTime;
			return;
		}

		if(timeSinceCheck <= checkIntervall)
		{
			timeSinceCheck += Time.deltaTime;
		}
		else
		{
			timeSinceCheck = 0f;
			
			foreach(RuneCloud runeCloud in runeClouds)
			{
				if(runeCloud.lifeTime >= minRuneCloudAge && runeCloud.pointCloudData.Count <= minimumPoints)
				{
					SendGlobal(GlobalEvent.RUNECLOUD_SELFDESTRUCT, new RuneCloudData(runeCloud));
				}
			}
		}
	}

	public void ReceiveGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null) 
	{
		switch(eventState)
		{
			case GlobalEvent.RUNECLOUD_SPAWNED:
			{ 
				if (globalSignalData is RuneCloudData data)
				{
					runeClouds.Add(data.runeCloud);
				}
				break;
			}
			case GlobalEvent.RUNECLOUD_DESTROYED:
			{
				if (globalSignalData is RuneCloudData data)
				{
					if (runeClouds.Count > 0)
					{
						foreach (RuneCloud runeCloud in runeClouds)
						{
							if (runeCloud == data.runeCloud)
							{
								//finds the RuneCloud in the list and removes it.
								runeClouds.Remove(runeCloud);
							}
						}
					}
				}
				break;
			}
		}
	}

	public void SendGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
	{
		GlobalMediator.Instance.ReceiveGlobal(eventState, globalSignalData);
	}
}