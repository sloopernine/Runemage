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
		
		return PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
	}
}