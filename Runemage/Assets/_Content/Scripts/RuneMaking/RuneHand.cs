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

	private MovingSound drawSound;

	private void Start()
	{
		GlobalMediator.Instance.Subscribe(this);
		if (usePCDraw && handPosition == null)
        {
			Debug.LogError("Can't use PCDraw witout a pc hand transfrom to the hand.");
        }

		drawSound = GetComponentInChildren<MovingSound>();
		drawSound.isPlaying = false;
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
		
		//if player is holding something, you cannot draw a new rune and falls out of update.
		if(handVR.currentAttachedObject != null)
		{
			return;
		}

		if (!isDrawing && isPressed)
		{
			StartMovement(handVR.skeleton.indexTip.position);
		}
		else if (isDrawing && !isPressed)
		{
			EndMovement();
		}
		else if (isDrawing && isPressed)
		{
			UpdateMovement(handVR.skeleton.indexTip.position);
		}
	}

	private void StartMovement(Vector3 position)
	{	
		isDrawing = true;


		runeCloud = Instantiate(prefabRuneCloud, position, Quaternion.identity).GetComponent<RuneCloud>();
		inRuneCloud = true;
		
		GenericSoundController.Instance.Play(WorldSounds.RuneDrawStart, transform.position, false);
		drawSound.isPlaying = true;
	}

	private void EndMovement()
	{
		isDrawing = false;

		if (inRuneCloud)
		{
			runeCloud.EndDraw();
			runeCloud = null;
			inRuneCloud = false;
		}
		
		drawSound.isPlaying = false;
	}

	private void UpdateMovement(Vector3 position)
	{
		if (inRuneCloud)
		{
			runeCloud.AddPoint(position);
		}
	}
	
	public void SetOutsideRuneCloud()
	{
		runeCloud = null;
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