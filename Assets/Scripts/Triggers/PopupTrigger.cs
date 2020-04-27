using UnityEngine;
using System.Collections;

public class PopupTrigger : MonoBehaviour 
{
	public string message;
	
	public SwitchUnitTrigger offSwitch;
	
	void Awake()
	{
		if (offSwitch)
		{
			offSwitch.SwitchTriggered += TurnOff;	
		}
	}
	
	void TurnOff()
	{
		GetComponent<Collider>().enabled = false;
		if (offSwitch)
		{
			offSwitch.SwitchTriggered -= TurnOff;	
		}
	}
	
	void OnTriggerEnter()
	{
		PopupGUI.Popup(message);
		GetComponent<Collider>().enabled = false;
	}
}
