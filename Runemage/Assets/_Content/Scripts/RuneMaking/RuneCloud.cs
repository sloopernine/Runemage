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

	private List<Vector3> newPointCloudData = new List<Vector3>();
	private List<Vector3> totalCloudPoints = new List<Vector3>();
	
	public GameObject subLineRendererPrefab;
	private List<LineRenderer> subLineRenderers = new List<LineRenderer>();

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
		newPointCloudData.Add(transform.position);
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
		Vector3 lastPoint = newPointCloudData[newPointCloudData.Count - 1];

		float cloudSize = GetCloudSize();
		
		trigger.radius = cloudSize * triggerSizeModifier;
		
		if (Vector3.Distance(point, lastPoint) > newPositionThresholdDistance)
		{
			newPointCloudData.Add(point);
			lineRenderer.positionCount = newPointCloudData.Count;
			lineRenderer.SetPosition(newPointCloudData.Count - 1, point);
		}
		else
		{
			lineRenderer.positionCount = newPointCloudData.Count;
			lineRenderer.SetPosition(newPointCloudData.Count - 1, point);
		}

		fadeCounter = fadeTime;
	}

	public void EndDraw()
	{
		if (newPointCloudData.Count > 2)
		{
	
			GameObject subLineRendrerGameObject = Instantiate(subLineRendererPrefab, transform);
			LineRenderer subLineRenderer = subLineRendrerGameObject.GetComponent<LineRenderer>();

			subLineRenderer.positionCount = newPointCloudData.Count;

			int index = 0;
			
			foreach (Vector3 point in newPointCloudData)
			{
				totalCloudPoints.Add(point);
				subLineRenderer.SetPosition(index, point);
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
		newPointCloudData.Clear();
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
		if (newPointCloudData.Count <= 2)
		{
			return triggerStartSize;
		}
		
		float distance = 0;
		Vector3 centroid = Vector3.zero;

		foreach (Vector3 point in newPointCloudData)
		{
			float newDistance = Vector3.Distance(transform.position, point);
			centroid += point;
			
			if (distance < newDistance)
			{
				distance = newDistance;
			}
		}

		centroid = centroid / newPointCloudData.Count;

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
		Point[] pointArray = new Point[newPointCloudData.Count];
		
		for (int i = 0; i < pointArray.Length; i++) 
		{
			Vector2 screenPoint = Camera.main.WorldToScreenPoint(newPointCloudData[i]);
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