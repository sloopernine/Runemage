using _Content.Scripts.Data.Containers.GlobalSignal;
using Data.Enums;
using Data.Interfaces;
using Singletons;
using UnityEngine;
using Valve.VR;

public class RuneHand : MonoBehaviour, IReceiveGlobalSignal
{
	
	private RuneCloud runeCloud;

	public GameObject prefabRuneCloud;

	private SpellCastOrigin spellCastOrigin;
	
	public SteamVR_Input_Sources inputSource;

	public SteamVR_Action_Pose poseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose");

	public SteamVR_Action_Boolean grabAction;

	public SteamVR_Action_Vector3 handPosition;

	public Transform PCHandPosition;

	[SerializeField] bool usePCDraw;

	private bool isPressed;
	
	private bool isDrawing;

	private bool inRuneCloud;

	private void Start()
	{
		GlobalMediator.Instance.Subscribe(this);
		if (usePCDraw && handPosition == null)
        {
			Debug.LogError("Can't use PCDraw witout a pc hand transfrom to the hand.");
        }
	}

	void Update()
	{
		if (!usePCDraw)
        {
			transform.position = poseAction[inputSource].localPosition;
			isPressed = grabAction.GetState(inputSource);
		}
		else
        {
			transform.position = PCHandPosition.position;
			isPressed = Input.GetMouseButton(0);
        }
		
		//this checks if you are inside of the "spellCastOrigin" by checking if it is null.
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
		else
		{
			runeCloud.InitStartMovement(false, transform.position);
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
		if (this.runeCloud != runeCloud)
		{
			EndMovement();
			SetOutsideRuneCloud();
		}
		else
		{
			this.runeCloud = runeCloud;
			inRuneCloud = true;
		}
	}

	public void SetOutsideRuneCloud()
	{
		this.runeCloud = null;
		inRuneCloud = false;
	}

	public void ReceiveGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
	{
		switch (eventState)
		{
			case GlobalEvent.RUNECLOUD_DESTROYED:

				if (globalSignalData is RuneCloudData data)
				{
					if (runeCloud == data.runeCloud)
					{
						SetOutsideRuneCloud();
					}
				}
				
				break;
		}
	}
}