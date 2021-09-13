using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDollarGestureRecognizer;
using System.IO;
using Valve.VR;

public class RuneHand : MonoBehaviour {
	
	private RuneCloud runeCloud;

	public GameObject prefabRuneCloud;
	
	public SteamVR_Input_Sources inputSource;

	public SteamVR_Action_Boolean grabAction;

	public SteamVR_Action_Vector3 handPosition;
	
	private bool isDrawing;

	private bool inRuneCloud;

	public Transform handTransform;
	
	void Update()
	{
		transform.position = handPosition[inputSource].axis; //.GetAxis(inputSource);
		
		bool isPressed = grabAction.GetState(inputSource);

		Debug.Log("isPressed: " + isPressed);
		
		if (!isDrawing && isPressed)
		{
			Debug.Log("START");
			StartMovement(transform.position);
		}
		else if (isDrawing && !isPressed)
		{
			Debug.Log("END");
			EndMovement();
		}
		else if (isDrawing && isPressed)
		{
			Debug.Log("UPDATE");
			UpdateMovement(transform.position);
		}
	}

	private void StartMovement(Vector3 position)
	{	
		isDrawing = true;
		
		if (runeCloud == null)
		{
			runeCloud = Instantiate(prefabRuneCloud, position, Quaternion.identity).GetComponent<RuneCloud>();
		}
	}

	private void EndMovement()
	{
		isDrawing = false;
		
		if (inRuneCloud)
		{
			runeCloud.EndDraw();
		}
	}

	private void UpdateMovement(Vector3 position)
	{
		if (inRuneCloud)
		{
			runeCloud.AddPoint(position);
		}
	}

	public void SetInRuneCloud(RuneCloud runeCloud)
	{
		this.runeCloud = runeCloud;
		inRuneCloud = true;
	}

	public void SetOutsideRuneCloud()
	{
		inRuneCloud = false;
	}
}