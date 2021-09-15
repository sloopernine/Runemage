using System;
using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer;
using System.IO;
using Valve.VR;

public class RuneChecker : MonoBehaviour
{
	private static RuneChecker instance;
	public static RuneChecker Instance
	{
		get => instance;
	}

	public bool trainingMode;

	public string newGestureName;
	private List<Gesture> trainingSet = new List<Gesture>();
	
	public List<Vector2> commonPointCloudList = new List<Vector2>();
	
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
		string[] gestureFiles = Directory.GetFiles(Application.persistentDataPath, "*.xml");
		
		foreach (var item in gestureFiles)
		{
			trainingSet.Add(GestureIO.ReadGestureFromFile(item));
		}
	}
	
	public void AddPointCloud(Point[] points)
	{
		Debug.Log("RuneChecker recieves pointCloud.");
		Gesture newGesture = new Gesture(points);

		newGesture.Name = newGestureName;
		
		if (trainingMode)
		{
			newGesture.Name = newGestureName;
			trainingSet.Add(newGesture);

			string path = Application.persistentDataPath + "/" + newGestureName + ".xml";
			GestureIO.WriteGesture(points, newGestureName, path);
		}
		else 
		{
			Debug.Log("RuneChecker approves of rune.");

			Result result = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
			Debug.Log(result.GestureClass + " " + result.Score);

			//RuneCloud, RuneChecker, RuneMaker, RuneHands, & SpellBall & Global Mediator.
			//RuneCloud RuneChecker is this spell or no?
			//if spell, then make this spell, (send spell type).
			//if not spell, begin to fade

			//RuneCloud, if get spelltype, then check how good, against own list,
			//Then talk to Global Mediator spawn spell here
			//Then kill itself.
		}
	}

	public Result Classify(Point[] points)
	{
		Gesture newGesture = new Gesture(points);

		Result returnResult = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());

		returnResult.spell = GetSpellEnum(returnResult.GestureClass);
		
		return PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
	}

	private Spell GetSpellEnum(string spellName)
	{
		string[] spellNames = Enum.GetNames(typeof(Spell));

		Spell returnValue = Spell.None;
		
		for (int i = 0; i < spellNames.Length; i++)
		{
			if (spellNames[i] == spellName)
			{
				returnValue = (Spell) i;
			}
		}
		
		return returnValue;
	}
}