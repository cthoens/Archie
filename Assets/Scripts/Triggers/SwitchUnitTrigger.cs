using UnityEngine;
using System.Collections;

public class SwitchUnitTrigger : MonoBehaviour 
{
	public PlayerController player;
	public SwitchUnitTarget target;
	public Material unlockedMaterial;
	
	public delegate void TriggeredDelegate();
	public TriggeredDelegate SwitchTriggered;

	private CameraMovement cameraMovement;
	private PointOfInterestCamera poiCamera;
	private enum Phases : byte { IDLE, WAITING_FOR_TURN, CAMERA_MOVING_IN, SWITCHING, CAMERA_MOVING_OUT, DISABLED };
	private Phases phase = Phases.IDLE;
	private Vector3 transformedFacing;
		
	void Awake()
	{
		cameraMovement = Camera.main.GetComponent<CameraMovement>();
		poiCamera = Camera.main.GetComponent<PointOfInterestCamera>();
		transformedFacing = transform.TransformDirection(Vector3.forward);
		enabled = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Phase one: Wait for the camera to point at the switch target
		switch ( phase )
		{
		case Phases.WAITING_FOR_TURN:
			if ( Vector3.Dot( transformedFacing, player.transform.forward ) >= 0.9999f )
			{
				enabled = false;
				DoSwitch();
			}
			break;
		}		
	}

	public void CameraReachedTarget()
	{
		switch (phase)
		{
		case Phases.CAMERA_MOVING_IN:
			target.Switch();
			if (SwitchTriggered != null)
			{
				SwitchTriggered();
			}
			transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = unlockedMaterial;
			phase = Phases.SWITCHING;
			target.switchingDone += CameraReachedTarget;
			break;

		case Phases.SWITCHING:
			target.switchingDone -= CameraReachedTarget;	
			poiCamera.target = player.transform;
			poiCamera.reverse();
			poiCamera.enabled = true;
			phase = Phases.CAMERA_MOVING_OUT;
			break;
		
		case Phases.CAMERA_MOVING_OUT:			
			cameraMovement.enabled = true;
			poiCamera.targetReached -= CameraReachedTarget;
			Controls.instance.enabled = true;
			phase = Phases.IDLE;
			break;
		}
	}
	
	public void DoSwitch() 
	{
		if ( !target.CanSwitch() )
		{
			return;
		}		

		Controls.SetButtonVisible(false);
		cameraMovement.enabled = false;
		poiCamera.target = target.transform;
		poiCamera.relativeCameraPos = target.relativeCameraPos;
		poiCamera.cameraDirection = target.cameraDirection;
		poiCamera.duration = target.cameraDuration;
		poiCamera.enabled = true;
		poiCamera.targetReached += CameraReachedTarget;
		phase = Phases.CAMERA_MOVING_IN;
	}

	// --- Trigger
	// -----------

	private void SwitchButtonPressed()
	{
		// remove the delegate
		Controls.OnSwitchButtonPressed -= SwitchButtonPressed;
		Controls.instance.enabled = false;
		// if the player is currently facing towards the button
		if ( Vector3.Dot( transformedFacing, player.transform.forward ) >= 0.9999f )
		{
			// ... start the switching cycle imediately
			DoSwitch();
		}
		else
		// if the player is NOT currently facing the button
		{
			phase = Phases.WAITING_FOR_TURN;
			enabled = true;
			Controls.desiredFacing = transformedFacing;
		}
	}
	
	void OnTriggerEnter( Collider other )
	{
		if ( other.isTrigger || phase != Phases.IDLE )
		{
			return;
		}

		if ( !target.CanSwitch() )
		{
			phase = Phases.DISABLED;
			GetComponent<Collider>().enabled = false;
			return;
		}		

		Controls.SetButtonVisible(true);
		Controls.OnSwitchButtonPressed += SwitchButtonPressed;
	}
	
	void OnTriggerExit( Collider other )
	{
		if ( other.isTrigger )
		{
			return;
		}

		Controls.OnSwitchButtonPressed -= SwitchButtonPressed;
		Controls.SetButtonVisible(false);
	}
}
