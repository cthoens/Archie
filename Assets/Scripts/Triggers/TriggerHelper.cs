using UnityEngine;
using System.Collections;

/**
 * Note: If a GameObject that has triggered a trigger is deactivated OnTriggerExit() is
 * not called. This is why we need to rely on OnTriggerStay()
 */
public class TriggerHelper : MonoBehaviour 
{
	private bool fIsTriggerd = false;
	private bool wasUpdated = false;
	public bool isTriggered
	{
		get { return fIsTriggerd; }
	}

	void OnTriggerEnter( Collider other )
	{
		if ( other.isTrigger )
		{
			return;
		}
		
		fIsTriggerd = true;
		wasUpdated = true;
	}
	
	void OnTriggerStay( Collider other )
	{
		if ( other.isTrigger )
		{
			return;
		}
		
		fIsTriggerd = true;
		wasUpdated = true;
	}

	void LateUpdate()
	{
		if ( fIsTriggerd && !wasUpdated )
		{
			fIsTriggerd = false;
		}
		wasUpdated = false;
	}
}
