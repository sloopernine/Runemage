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
		
		TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/");

		foreach (TextAsset gestureXml in gesturesXml)
		{
			trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));
		}
		
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
		}
	}

	public Result Classify(Point[] points)
	{
		Gesture newGesture = new Gesture(points);

		Result returnResult = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
		Debug.Log("returnResult is: " + returnResult.GestureClass);

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
				Debug.Log("Spellnames is: " + spellNames[i] + " and the current spell is: " + spellName);

				returnValue = (Spell) i;
			}
		}
		
		return returnValue;
	}
}