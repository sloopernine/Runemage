using System.Collections;
using System.Collections.Generic;
using PDollarGestureRecognizer;
using UnityEngine;
using Data.Interfaces;
using Singletons;
using Data.Enums;
using _Content.Scripts.Data.Containers.GlobalSignal;

public class RuneCloud : MonoBehaviour, ISendGlobalSignal, IReceiveGlobalSignal
{
	private LineRenderer lineRenderer;
	private SphereCollider trigger;

	public float newPositionThresholdDistance;

	public List<Vector3> newLinePointCloudData = new List<Vector3>();
	public List<Vector3> totalCloudPoints = new List<Vector3>();

	public GameObject subLineRendererPrefab;

	//private Result result;

	public float triggerStartSize;
	public float triggerSizeModifier;

	public float spellballSize;

	[Header("LifeSpan")]
	[Min(0f)] public float lifeTime;
	[Min(0.1f)] private float lifetimeLeft;
	public float lifeSpan;

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

		InitStartMovement(true, Vector3.zero);
		
		triggerStartSize = trigger.radius / 2;

		lifetimeLeft = lifeSpan;
		
		SendGlobal(GlobalEvent.RUNECLOUD_SPAWNED, new RuneCloudData(this));
		GlobalMediator.Instance.Subscribe(this);
	}

	private void Update()
	{
		lifeTime += Time.deltaTime;
		
		if(lifeTime >= lifetimeLeft)
		{
			isFading = true;
			StartCoroutine(FadeRune());
		}
	}

	public void InitStartMovement(bool firstInit, Vector3 point)
	{
		if (firstInit)
		{
			newLinePointCloudData.Add(transform.position);
			lineRenderer.positionCount = 1;
			lineRenderer.SetPosition(0, transform.position);
		}
		else
		{
			newLinePointCloudData.Add(point);
			totalCloudPoints.Add(point);
		}
	}

	private IEnumerator FadeRune()
	{
		Debug.Log("FadeCounter Started");
		yield return new WaitForSeconds(fadeTime);
		Debug.Log("Done waiting to Destroy");
		DestroyRuneCloud();
	} 

	public void AddPoint(Vector3 point)
	{
		if (isFading)
		{
			return;
		}
		
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
		
		if(trigger.radius < 0.08f)
		{
			trigger.radius = 0.2f;
		}
	}

	public void EndDraw()
	{
		lifetimeLeft = lifeSpan + lifeTime;
		
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
				Result result = RuneChecker.Instance.Classify(pointArray);

				ValidateSpell(result);
			}
		}
		
		lineRenderer.positionCount = 0;
		newLinePointCloudData.Clear();
	}

	private void ValidateSpell(Result result)
	{
		if (result.spell != Spell.None)
		{
			SendGlobal(GlobalEvent.CREATE_SPELL, new RuneData(result, centroidPosition));
			StopAllCoroutines();
			DestroyRuneCloud();
		}
		else
		{
			Debug.Log("No matching rune");
			isFading = true;
		}
	}

	private void DestroyRuneCloud()
	{
		SendGlobal(GlobalEvent.RUNECLOUD_DESTROYED, new RuneCloudData(this));
		Destroy(gameObject);
	}

	private float GetCloudSize()
	{
		if (totalCloudPoints.Count <= 2)
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
		if (isFading)
		{
			return;
		}
		
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

	public void ReceiveGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
	{
		switch (eventState)
		{
			case GlobalEvent.RUNECLOUD_SELFDESTRUCT:
			{
				if (globalSignalData is RuneCloudData data)
				{
					if (this == data.runeCloud)
					{
						this.DestroyRuneCloud();
					}
				}
				break;
			}
		}
	}
}