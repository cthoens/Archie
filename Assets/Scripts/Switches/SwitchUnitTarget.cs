using UnityEngine;
using System.Collections;

public abstract class SwitchUnitTarget : MonoBehaviour
{
	public float cameraDuration = 3.0f;
	public Vector3 relativeCameraPos;   
	public Vector3 cameraDirection;

	public delegate void switchingDoneDelegate();
	public switchingDoneDelegate switchingDone;
	
	protected bool didSwitch = false;
	
	public virtual bool CanSwitch()
	{
		return !didSwitch;
	}
	
	public virtual bool IsSwitchingDone()
	{
		return true;
	}
	
	public abstract void Switch();

	void OnDrawGizmosSelected() 
	{		
		Gizmos.color = Color.magenta;
		Vector3 position = transform.position + transform.TransformDirection( relativeCameraPos );
		Gizmos.DrawWireSphere( position, 0.1f );
		Gizmos.DrawLine(position, position + transform.TransformDirection( cameraDirection ) );
	}
}

