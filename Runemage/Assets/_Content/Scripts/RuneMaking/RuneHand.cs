using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer;
using System.IO;
using Valve.VR;

public class RuneHand : MonoBehaviour
{
	public SteamVR_Input_Sources handInput;

	public SteamVR_Action_Boolean grabAction;

	public SteamVR_Action_Vector3 handPosition;

	private bool isMoving;

	public float newPositionThresholdDistance;
	public List<Vector3> pointCloudList = new List<Vector3>();
	
	void Update() 
	{
		bool isPressed = grabAction.GetState(handInput);

		if (!isMoving && isPressed)
		{
			StartMovement(handPosition.GetAxis(handInput));
		}
		else if (isMoving && !isPressed)
		{
			EndMovement(handPosition.GetAxis(handInput));
		}
		else if (isMoving && isPressed)
		{
			UpdateMovement(handPosition.GetAxis(handInput));
		}
	}

	private void StartMovement(Vector3 position)
	{	
		isMoving = true;
		pointCloudList.Clear();
		pointCloudList.Add(position);
	}

	private void EndMovement(Vector3 position)
	{
		isMoving = false;
		Point[] pointArray = new Point[pointCloudList.Count];

		for (int i = 0; i < pointArray.Length; i++) {
			Vector2 screenPoint = Camera.main.WorldToScreenPoint(pointCloudList[i]);
			pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
		}

		//Point cloud sends to RuneMaker without being made into a Gesture.
	}

	private void UpdateMovement(Vector3 position)
	{
		Vector3 lastPoint = pointCloudList[pointCloudList.Count - 1];

		if (Vector3.Distance(position, lastPoint) > newPositionThresholdDistance)
		{
			pointCloudList.Add(position);
			// lineRenderer.positionCount = pointCloudList.Count;
			// lineRenderer.SetPosition(pointCloudList.Count - 1, position);
		} 
		else
		{
			// lineRenderer.positionCount = pointCloudList.Count;
			// lineRenderer.SetPosition(pointCloudList.Count - 1, position);
		}
	}
}