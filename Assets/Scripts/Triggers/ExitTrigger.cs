using UnityEngine;
using System.Collections;

public class ExitTrigger : MonoBehaviour 
{
	public PlayerController player;
	public string exitMessage = "Hurra, you have found the exit! \n\n Thank you for testing my game!";

	void Awake()
	{
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.isTrigger)
		{
			return;
		}

		Camera.main.GetComponent<CameraMovement>().enabled = false;
		PopupGUI.Popup(exitMessage, PopupClosed);
	}

	void PopupClosed()
	{
		MenuGUI.instance.enabled = true;
	}
}