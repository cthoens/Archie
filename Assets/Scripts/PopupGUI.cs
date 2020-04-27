using UnityEngine;
using System.Collections;

public class PopupGUI : MonoBehaviour 
{	
	public PlayerController player;	
	public float height = 300;
	public GUIStyle areaStyle = new GUIStyle();
	public GUIStyle labelStyle = new GUIStyle();
	public GUIStyle closeButtonStyle = new GUIStyle();
	
	private string message;
	private static PopupGUI instance;
	public delegate void PopupCloseDelegate();
	PopupCloseDelegate closedDelegate;
	
	void Awake ()
	{
		instance = this;
		enabled = false;
	}
	
	void FixedUpdate()
	{
		if ( Input.GetButton("Dismiss") || Controls.TouchBegan() )
		{
			closePopup();
		}
	}
	
	// Update is called once per frame
	void OnGUI() 
	{
		// Make a group on the center of the screen
		GUILayout.BeginArea( new Rect( Screen.width / 2 - 250, (Screen.height - height) / 2, 500, height ), areaStyle );
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if ( GUILayout.Button("", closeButtonStyle) )
		{
			closePopup();
		}
		GUILayout.EndHorizontal();
		GUILayout.Label( message, labelStyle, GUILayout.ExpandHeight(true) );
		GUILayout.EndArea();
	}

	private void closePopup()
	{
		enabled = false;
		Controls.instance.enabled = true;
		player.stopping = false;
		message = null;
		if (closedDelegate != null)
		{
			closedDelegate();
		}
	}
	
	public static void Popup(string message)
	{
		Popup(message, null);
	}

	public static void Popup(string message, PopupCloseDelegate closeDelegate)
	{
		instance.closedDelegate = closeDelegate;
		instance.message = message;
		instance.enabled = true;
		instance.player.stopping = true;
		Controls.instance.enabled = false;
		Controls.keyPushed = false;
	}
}
