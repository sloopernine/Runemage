using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer;
using System.IO;
using Valve.VR;

public class RuneMaker : MonoBehaviour
{
	public bool trainingMode;

	public string newGestureName;
	private List<Gesture> trainingSet = new List<Gesture>();
	
	public float newPositionThresholdDistance;
	public List<Vector2> commonPointCloudList = new List<Vector2>();
	
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

			//Earlier we said that the runeCloud should spawn the spell, not the RuneMaker
			//So, if the rune is "approved" should this method then return something?
			//Either a yes or a no right?
			//But does that become a problem if/when we are training? Seeing as a spell is not returned then?
			//So, we need to return something here, so you know if the spell is succ or if it should 
		}
	}
}