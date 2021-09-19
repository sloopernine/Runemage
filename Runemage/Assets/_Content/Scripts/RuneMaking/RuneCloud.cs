using System.Collections.Generic;
using _Content.Scripts.Data.Containers;
using PDollarGestureRecognizer;
using UnityEngine;
using Data.Interfaces;
using Singletons;
using Data.Enums;
using _Content.Scripts.Data.Containers.GlobalSignal;

public class RuneCloud : MonoBehaviour, ISendGlobalSignal
{
	private LineRenderer lineRenderer;
	private SphereCollider trigger;

	public float newPositionThresholdDistance;

	private List<Vector3> pointCloud = new List<Vector3>();

	public GameObject subCloudPrefab;
	private List<SubCloud> subClouds = new List<SubCloud>();

	private Result result;

	public float triggerStartSize;
	public float triggerSizeModifier;
	public float spellballSize;
	
	public float spellThreshold = 0f;
	public float fadeTime;
	private float fadeCounter;
	private Vector3 centroidPosition;
	
	private GameManager gameManager = GameManager.Instance;
	[Header("Gesture Training")]
	public string gestureName;
	
	private void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		trigger = GetComponent<SphereCollider>();

		lineRenderer.positionCount = 1;
		pointCloud.Add(transform.position);
		lineRenderer.SetPosition(0, transform.position);

		fadeCounter = fadeTime;
		triggerStartSize = trigger.radius / 2;
	}

	private void Update()
	{
		if (fadeCounter > 0)
		{
			fadeCounter -= Time.deltaTime;
		}
		else
		{
			DestroyRuneCloud();
		}
	}

	public void AddPoint(Vector3 point)
	{
		Vector3 lastPoint = pointCloud[pointCloud.Count - 1];

		float cloudSize = GetCloudSize();
		
		trigger.radius = cloudSize * triggerSizeModifier;
		
		if (Vector3.Distance(point, lastPoint) > newPositionThresholdDistance)
		{
			pointCloud.Add(point);
			lineRenderer.positionCount = pointCloud.Count;
			lineRenderer.SetPosition(pointCloud.Count - 1, point);
		}
		else
		{
			lineRenderer.positionCount = pointCloud.Count;
			lineRenderer.SetPosition(pointCloud.Count - 1, point);
		}

		fadeCounter = fadeTime;
	}

	public void EndDraw()
	{
		if (pointCloud.Count > 2)
		{
	
			GameObject subCloudGameObject = Instantiate(subCloudPrefab, transform);
			SubCloud subCloud = subCloudGameObject.GetComponent<SubCloud>();
		
			subClouds.Add(subCloud);
			subCloud.AddPoints(pointCloud);
			
			List<Vector3> totalCloudPoints = new List<Vector3>();
			
			foreach (var cloud in subClouds)
			{
				foreach (var point in cloud.GetPointList())
				{
					totalCloudPoints.Add(point);
				}
			}
			
			Point[] pointArray = new Point[totalCloudPoints.Count];
			
			for (int i = 0; i < pointArray.Length; i++) 
			{
				Vector2 screenPoint = Camera.main.WorldToScreenPoint(totalCloudPoints[i]);
				pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
			}
			
			if (!gameManager.gestureTrainingMode) 
			{
				result = RuneChecker.Instance.Classify(pointArray);
				//Debug.Log("Result name: " + result.GestureClass);
				//Debug.Log("Result score: " + result.Score);
				ValidateSpell();
			}
		}

		lineRenderer.positionCount = 0;
		pointCloud.Clear();
	}

	private void ValidateSpell()
	{
		//TODO: Turn into switch here if use indivudual spellThresholdvalue.
		if (result.Score >= spellThreshold)
		{
			SendGlobal(GlobalEvent.CREATE_SPELL_ORIGIN, new RuneData(result, centroidPosition, transform.eulerAngles, new Vector3(spellballSize, spellballSize, spellballSize)));
			DestroyRuneCloud();
		}
		else
		{
			
			//TODO do we want to give feedback on too low threshold?
			//And begin fade of spell?
		}
	}

	private float GetCloudSize()
	{
		if (pointCloud.Count <= 2)
		{
			return triggerStartSize;
		}
		
		float distance = 0;
		Vector3 centroid = Vector3.zero;

		foreach (Vector3 point in pointCloud)
		{
			float newDistance = Vector3.Distance(transform.position, point);
			centroid += point;
			
			if (distance < newDistance)
			{
				distance = newDistance;
			}
		}

		centroid = centroid / pointCloud.Count;

		centroidPosition = centroid;
		
		trigger.center = transform.InverseTransformPoint(centroid);
		
		return distance;
	}

	private void DestroyRuneCloud()
	{
		SendGlobal(GlobalEvent.RUNECLOUD_DESTROYED, new RuneCloudData(this));
		Destroy(gameObject);
	}

	public void SaveGestureToXML()
	{
		Point[] pointArray = new Point[pointCloud.Count];
		
		for (int i = 0; i < pointArray.Length; i++) 
		{
			Vector2 screenPoint = Camera.main.WorldToScreenPoint(pointCloud[i]);
			pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
		}
	
		Gesture newGesture = new Gesture(pointArray);
	
		newGesture.Name = gestureName;
		
		string path = Application.dataPath + "/_Content/Resources/GestureSet/" + gestureName + ".xml";
		GestureIO.WriteGesture(pointArray, gestureName, path);
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("RuneHand"))
		{
			other.GetComponent<RuneHand>().SetInRuneCloud(this);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("RuneHand"))
		{
			other.GetComponent<RuneHand>().SetOutsideRuneCloud();
		}
	}

	public void SendGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
	{
		GlobalMediator.Instance.ReceiveGlobal(eventState, globalSignalData);
	}
}