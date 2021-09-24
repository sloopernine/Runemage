using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The purpose of this script is to periodically look through a scene,
/// find all of the runeclouds in that scene
/// (I don't trust the runeclouds to subscribe on their own, since they seems to be breaking)
/// add them to a list.
/// (wait, does it need to go through the list then and first find all, and then check if it already have them on the list?
/// even though we might never have more than 10 runeclouds at any given time that seems scary to have that many searches happening every third second or so...)
/// then it should wait
/// then go through the list, look at each runecloud,
/// if it still exists && has too few points && haven't gotten any more points since last time.
/// Destroy it.
/// The check of how many points it had last time might be difficult, how do you pair that up so that it refers to the correct runecloud if
/// the list shrinks and grows?
/// Should it be a public var on that runecloud?
/// Should this object contain instances of a kind of objects, a new kind, that contains a reference to the runecloud and a var that it sets the first time it checks?
/// 
/// This class will most likely need to be a singleton. Because there could be a lot of problems if you have more than one of them in a scene.
/// 
/// Also, it needs to tell the RuneClouds to destroy themselves using a message sent to the global mediator.
/// </summary>
public class RuneDestroyer : MonoBehaviour 
{

	[SerializeField] [Min(0.1f)] float checkIntervall;
	private float timeSinceCheck;
	[SerializeField] [Min(0)] int pointMinimum;

	private List<RuneCloud> runeClouds;

	void Start()
	{

	}

	void Update()
	{
		if(timeSinceCheck <= checkIntervall)
		{
			timeSinceCheck += Time.deltaTime;
		}
		else
		{
			
		}
	}
}
