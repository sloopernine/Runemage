using _Content.Scripts.Data.Containers.GlobalSignal;
using Data.Enums;
using Data.Interfaces;
using Singletons;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class RuneHand : MonoBehaviour, IReceiveGlobalSignal {

	[Tooltip("The VR-hand reference, check so that left is connected to left & right to right")]
	[SerializeField] Hand handVR;

	private RuneCloud runeCloud;

	public GameObject prefabRuneCloud;

	public SteamVR_Input_Sources inputSource;

	public SteamVR_Action_Pose poseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose");

	public SteamVR_Action_Boolean grabAction;

	public SteamVR_Action_Vector3 handPosition;

	public Transform PCHandPosition;

	[SerializeField] bool usePCDraw;

	[Tooltip("if the grab-action is true")]
	private bool isPressed;

	[Tooltip("if the hand is already drawing a rune, or if it should spawn a new runecloud")]
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
			//Okay, so first
			//Then decide what button holds something,
			//If you are holding, then you can't draw
			//Then change so another button is making the drawing so that it is "point with index to draw" basically.
		}
		else
        {
			transform.position = PCHandPosition.position;
			isPressed = Input.GetMouseButton(0);
        }
		
		//if player is holding something, you cannot draw a new rune and falls out of update.
		if(handVR.currentAttachedObject != null)
		{
			return;
		}

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