using UnityEngine;
using Valve.VR;

public class RuneHand : MonoBehaviour {
	
	private RuneCloud runeCloud;

	public GameObject prefabRuneCloud;
	
	public SteamVR_Input_Sources inputSource;

	public SteamVR_Action_Pose poseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose");

	public SteamVR_Action_Boolean grabAction;

	public SteamVR_Action_Vector3 handPosition;
	
	private bool isDrawing;

	private bool inRuneCloud;

	void Update()
	{
		//Add on the SpellCastOrigin, triggers, that checks if it is RuneHands that are entering into it.
		//If they are, the SoellCastOrigin sets itself to something.
		//That something is checked here in Update, and it it *isn't* null, then you can't paint, but can instead move and kick of the spell.

		//(Later on I can add something about how, if your painting maybe move rune etc.)

		transform.position = poseAction[inputSource].localPosition;
		
		bool isPressed = grabAction.GetState(inputSource);
		
		if (!isDrawing && isPressed)
		{
			StartMovement(transform.position);
		}
		else if (isDrawing && !isPressed)
		{
			EndMovement();
		}
		else if (isDrawing && isPressed)
		{
			UpdateMovement(transform.position);
		}
	}

	private void StartMovement(Vector3 position)
	{	
		isDrawing = true;
		
		if (runeCloud == null)
		{
			runeCloud = Instantiate(prefabRuneCloud, position, Quaternion.identity).GetComponent<RuneCloud>();
			inRuneCloud = true;
		}
	}

	private void EndMovement()
	{
		isDrawing = false;

		Debug.Log("RuneHand enters EndMovement()");

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
		this.runeCloud = null;
		inRuneCloud = false;
	}
}