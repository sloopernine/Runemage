using System;
using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer;

public class RuneChecker : MonoBehaviour
{
	private static RuneChecker instance;
	public static RuneChecker Instance
	{
		get => instance;
	}
	
	public float SpellTreshold 
	{ 
	get => spellTreshold;
	}

	[SerializeField] float spellTreshold;
	
	private List<Gesture> trainingSet = new List<Gesture>();


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
	}
	
	public Result Classify(Point[] points)
	{
		Gesture newGesture = new Gesture(points);

		Result returnResult = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
		Debug.Log("Return result is: " + returnResult.GestureClass + " " + returnResult.Score + "%");

		returnResult.spell = GetSpellEnum(returnResult.GestureClass, returnResult.Score);
		
		return returnResult;
	}

	private Spell GetSpellEnum(string spellName, float score)
	{
		if (score <= spellTreshold)
		{
			return Spell.None;
		}
		
		string[] spellNames = Enum.GetNames(typeof(Spell));

		Spell returnValue = Spell.None;
		
		for (int i = 0; i < spellNames.Length; i++)
		{
			if (spellName.Contains(spellNames[i]))
			{
				returnValue = (Spell) i;
			}
		}
		
		return returnValue;
	}
}