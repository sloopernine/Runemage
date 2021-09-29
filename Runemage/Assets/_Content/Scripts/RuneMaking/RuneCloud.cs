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
	private Camera cameraMain;

	public float newPositionThresholdDistance;
	public int minimumPoints = 2;

	public List<Vector3> pointCloudData = new List<Vector3>();
	
	[Header("LifeSpan")]
	[Min(0f)] public float lifeTime;
	[Min(0.1f)] private float lifetimeLeft;
	public float lifeSpan;

	[SerializeField] float fadeTime;
	private bool isFading;

	private GameManager gameManager = GameManager.Instance;
	[Header("Gesture Training")]
	public string gestureName;
	
	private void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		cameraMain = Camera.main;

		InitStartMovement();
		
		lifetimeLeft = lifeSpan;
		
		SendGlobal(GlobalEvent.RUNECLOUD_SPAWNED, new RuneCloudData(this));
		GlobalMediator.Instance.Subscribe(this);
	}

	private void Update()
	{
		lifeTime += Time.deltaTime;
		
		if(lifeTime >= lifetimeLeft && !gameManager.gestureTrainingMode)
		{
			isFading = true;
			//StartCoroutine(FadeRune()); Coroutine seems to not work properly, destroys at once for now instead
			DestroyRuneCloud();
		}
	}

	public void InitStartMovement()
	{
		pointCloudData.Add(transform.position);
		lineRenderer.positionCount = 1;
		lineRenderer.SetPosition(0, transform.position);
	}

	private IEnumerator FadeRune()
	{
		Debug.Log("FadeCounter Started");
		
		Material lineMaterial = lineRenderer.material;
		
		float alpha = 1f;

		while (alpha > 0f)
		{
			lineMaterial.color = new Color(lineMaterial.color.r, lineMaterial.color.g, lineMaterial.color.b, alpha);
			
			yield return new WaitForSeconds(fadeTime);
			
			alpha -= 0.05f;
		}
		
		Debug.Log("Done waiting to Destroy");
		DestroyRuneCloud();
	} 

	public void AddPoint(Vector3 point)
	{
		if (isFading)
		{
			return;
		}
		
		lifetimeLeft = lifeSpan + lifeTime;
		
		Vector3 lastPoint = pointCloudData[pointCloudData.Count - 1];
		
		if (Vector3.Distance(point, lastPoint) > newPositionThresholdDistance)
		{
			pointCloudData.Add(point);
			lineRenderer.positionCount = pointCloudData.Count;
			
			lineRenderer.SetPosition(0, pointCloudData[0]);
			lineRenderer.SetPosition(pointCloudData.Count - 1, point);
		}
		else
		{
			lineRenderer.positionCount = pointCloudData.Count;
			lineRenderer.SetPosition(pointCloudData.Count - 1, point);
		}
	}

	public void EndDraw()
	{
		if (pointCloudData.Count <= minimumPoints)
		{
			DestroyRuneCloud();
		}
		else
		{
			Point[] pointArray = new Point[pointCloudData.Count];
			
			for (int i = 0; i < pointArray.Length; i++) 
			{
				Vector2 screenPoint = cameraMain.WorldToScreenPoint(pointCloudData[i]);
				pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
			}
			
			if (!gameManager.gestureTrainingMode) 
			{
				Result result = RuneChecker.Instance.Classify(pointArray);

				ValidateSpell(result);
			}
		}
	}

	private void ValidateSpell(Result result)
	{
		if (result.spell != Spell.None)
		{
			GenericSoundController.Instance.Play(WorldSounds.RuneDrawSuccess, transform.position, false);
			
			Vector3 centroidPosition = Vector3.zero;

			foreach (var point in pointCloudData)
			{
				centroidPosition += point;
			}
			
			centroidPosition = centroidPosition / pointCloudData.Count;
			
			SendGlobal(GlobalEvent.CREATE_SPELL, new RuneData(result, centroidPosition));
			DestroyRuneCloud();
		}
		else
		{
			GenericSoundController.Instance.Play(WorldSounds.RuneDrawFailure, transform.position, false);
			Debug.Log("No matching rune");
			isFading = true;
			DestroyRuneCloud();
		}
	}

	private void DestroyRuneCloud()
	{
		SendGlobal(GlobalEvent.RUNECLOUD_DESTROYED, new RuneCloudData(this));
		Destroy(gameObject);
	}
	
	public void SaveGestureToXML()
	{
		Point[] pointArray = new Point[pointCloudData.Count];
		
		for (int i = 0; i < pointArray.Length; i++) 
		{
			Vector2 screenPoint = Camera.main.WorldToScreenPoint(pointCloudData[i]);
			pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
		}
	
		Gesture newGesture = new Gesture(pointArray);
	
		newGesture.Name = gestureName;
		
		string path = Application.dataPath + "/_Content/Resources/GestureSet/" + gestureName + ".xml";
		GestureIO.WriteGesture(pointArray, gestureName, path);
		
		DestroyRuneCloud();
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
			
			case GlobalEvent.RUNECLOUD_DESTROYALL:

				DestroyRuneCloud();
				
				break;
		}
	}
}