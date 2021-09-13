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
	
	void Update() 
	{

		bool isPressed = grabAction.GetState(inputSource);

		if (!isDrawing && isPressed)
		{
			StartMovement(handPosition.GetAxis(inputSource));
		}
		else if (isDrawing && !isPressed)
		{
			EndMovement();
		}
		else if (isDrawing && isPressed)
		{
			UpdateMovement(handPosition.GetAxis(inputSource));
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