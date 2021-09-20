using System.Collections;
using System.Collections.Generic;
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

	private List<Vector3> pointCloudList = new List<Vector3>();

	private Result result;

	public float triggerStartSize;
	public float triggerSizeModifier;
	
	public float spellThreshold = 0f;
	
	[SerializeField] float fadeTime;
	private Vector3 centroidPosition;
	private bool isFading;

	private GameManager gameManager = GameManager.Instance;
	[Header("Gesture Training")]
	public string gestureName;
	
	private void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		trigger = GetComponent<SphereCollider>();

		lineRenderer.positionCount = 1;
		pointCloudList.Add(transform.position);
		lineRenderer.SetPosition(0, transform.position);

		triggerStartSize = trigger.radius / 2;
	}

	private void Update()
	{
		if(isFading)
		{
			StartCoroutine(FadeCounter(fadeTime));
		}
		
	}

	private IEnumerator FadeCounter(float fadeTime)
	{
		isFading = false;
		Debug.Log("FadeCounter Started");
		yield return new WaitForSeconds(fadeTime);
		Debug.Log("Done waiting to Destroy");
		Destroy(this.gameObject);
	} 

	public void AddPoint(Vector3 point)
	{
		StopAllCoroutines();

		Vector3 lastPoint = pointCloudList[pointCloudList.Count - 1];

		float cloudSize = GetCloudSize();
		
		trigger.radius = cloudSize * triggerSizeModifier;
		
		if (Vector3.Distance(point, lastPoint) > newPositionThresholdDistance)
		{
			pointCloudList.Add(point);
			lineRenderer.positionCount = pointCloudList.Count;
			lineRenderer.SetPosition(pointCloudList.Count - 1, point);
		}
		else
		{
			lineRenderer.positionCount = pointCloudList.Count;
			lineRenderer.SetPosition(pointCloudList.Count - 1, point);
		}

	}

	public void EndDraw()
	{
		isFading = true;

		Debug.Log("RuneCloud enters EndDraw()");

		Point[] pointArray = new Point[pointCloudList.Count];
		
		for (int i = 0; i < pointArray.Length; i++) 
		{
			Vector2 screenPoint = Camera.main.WorldToScreenPoint(pointCloudList[i]);
			pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
		}
		
		if (!gameManager.gestureTrainingMode && pointCloudList.Count > 2) // <- Prevent to few points to be classified, throws error if few
		{
			result = RuneChecker.Instance.Classify(pointArray);
			ValidateSpell();
		}
	}

	private void ValidateSpell()
	{
		
	//TODO: Turn into switch here if use indivudual spellThresholdvalue.
		if (result.Score >= spellThreshold)
		{
			Debug.Log("RuneHand says spell is above spellThreshold.");

			Debug.Log("RuneCloud sends CREATE_SPELL to Global Mediator.");
			SendGlobal(GlobalEvent.CREATE_SPELL_ORIGIN, new RuneData(result, transform.position, transform.eulerAngles, transform.localScale));
			Debug.Log("RuneCloud destroys itself.");
			Destroy(this.gameObject);

		}
		else
		{
			isFading = true;
		}
	}

	private float GetCloudSize()
	{
		if (pointCloudList.Count <= 2)
		{
			return triggerStartSize;
		}
		
		float distance = 0;
		Vector3 centroid = Vector3.zero;

		foreach (Vector3 point in pointCloudList)
		{
			float newDistance = Vector3.Distance(transform.position, point);
			centroid += point;
			
			if (distance < newDistance)
			{
				distance = newDistance;
			}
		}

		centroid = centroid / pointCloudList.Count;
		trigger.center = transform.InverseTransformPoint(centroid);
		
		return distance;
	}

	public void SaveGestureToXML()
	{
		Point[] pointArray = new Point[pointCloudList.Count];
		
		for (int i = 0; i < pointArray.Length; i++) 
		{
			Vector2 screenPoint = Camera.main.WorldToScreenPoint(pointCloudList[i]);
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