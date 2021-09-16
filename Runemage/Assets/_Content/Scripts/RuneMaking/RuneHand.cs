using UnityEngine;
using Valve.VR;

public class RuneHand : MonoBehaviour {
	
	private RuneCloud runeCloud;

	public GameObject prefabRuneCloud;

	private SpellCastOrigin spellCastOrigin;
	
	public SteamVR_Input_Sources inputSource;

	public SteamVR_Action_Pose poseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose");

	public SteamVR_Action_Boolean grabAction;

	public SteamVR_Action_Vector3 handPosition;
	
	private bool isDrawing;

	private bool inRuneCloud;

	void Update()
	{
		transform.position = poseAction[inputSource].localPosition;
		
		bool isPressed = grabAction.GetState(inputSource);
		
		//this checks if you are inside of the "spellCastOrigin" by checking if it is null.
		//TODO: Check how this interacts with the fact that you may be drawing inside of a rune, that is expanding.
		if(spellCastOrigin) 
		{
			if(isPressed)
			{
				//Sends the hands transform.position, might later on want to replace this with the players transform
				spellCastOrigin.CastSpell(transform.position);
			}
		}
		else
		{
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

	public void SetInSpellCastOrigin(SpellCastOrigin spellCastOrigin)
	{
		this.spellCastOrigin = spellCastOrigin;
	}

	public void SetOutsideSpellCastOrigin()
	{
		this.spellCastOrigin = null;
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