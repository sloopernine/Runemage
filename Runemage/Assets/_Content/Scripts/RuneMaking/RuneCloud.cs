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

	private List<Vector3> newLinePointCloudData = new List<Vector3>();
	private List<Vector3> totalCloudPoints = new List<Vector3>();
	
	public GameObject subLineRendererPrefab;

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

		InitDraw(true, Vector3.zero);
		
		triggerStartSize = trigger.radius / 2;
	}

	private void Update()
	{
		if (fadeCounter > 0 && !gameManager.gestureTrainingMode)
		{
			fadeCounter -= Time.deltaTime;
		}
		else
		{
			DestroyRuneCloud();
		}
	}

	public void InitDraw(bool firstInit, Vector3 point)
	{
		fadeCounter = fadeTime;

		if (firstInit)
		{
			newLinePointCloudData.Add(transform.position);
			lineRenderer.positionCount = 1;
			lineRenderer.SetPosition(0, transform.position);
		}
		else
		{
			newLinePointCloudData.Add(point);
		}
	}

	public void AddPoint(Vector3 point)
	{
		Vector3 lastPoint = newLinePointCloudData[newLinePointCloudData.Count - 1];

		if (Vector3.Distance(point, lastPoint) > newPositionThresholdDistance)
		{
			newLinePointCloudData.Add(point);
			totalCloudPoints.Add(point);
			lineRenderer.positionCount = newLinePointCloudData.Count;
			
			lineRenderer.SetPosition(0, newLinePointCloudData[0]);
			lineRenderer.SetPosition(newLinePointCloudData.Count - 1, point);
		}
		else
		{
			lineRenderer.positionCount = newLinePointCloudData.Count;
			lineRenderer.SetPosition(newLinePointCloudData.Count - 1, point);
		}
	
		float cloudSize = GetCloudSize();
		
		trigger.radius = cloudSize * triggerSizeModifier;

		fadeCounter = fadeTime;
	}

	public void EndDraw()
	{
		if (newLinePointCloudData.Count > 2)
		{
			GameObject subLineRendererGameObject = Instantiate(subLineRendererPrefab, transform);
			LineRenderer subLineRenderer = subLineRendererGameObject.GetComponent<LineRenderer>();

			subLineRenderer.positionCount = newLinePointCloudData.Count;

			int index = 0;
			
			foreach (Vector3 point in newLinePointCloudData)
			{
				subLineRenderer.SetPosition(index, point);
				index++;
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
		newLinePointCloudData.Clear();
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
		if (newLinePointCloudData.Count <= 2)
		{
			return triggerStartSize;
		}
		
		float distance = 0;
		Vector3 centroid = Vector3.zero;

		foreach (Vector3 point in totalCloudPoints)
		{
			float newDistance = Vector3.Distance(transform.position, point);
			centroid += point;
			
			if (distance < newDistance)
			{
				distance = newDistance;
			}
		}

		centroid = centroid / totalCloudPoints.Count;

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
		Point[] pointArray = new Point[newLinePointCloudData.Count];
		
		for (int i = 0; i < pointArray.Length; i++) 
		{
			Vector2 screenPoint = Camera.main.WorldToScreenPoint(newLinePointCloudData[i]);
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