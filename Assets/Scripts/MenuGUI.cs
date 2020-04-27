using UnityEngine;
using System.Collections;

public class MenuGUI : MonoBehaviour 
{
	public GUIStyle areaStyle = new GUIStyle();
	public GUIStyle buttonStyle = new GUIStyle();
	public GUIStyle buttonDisabledStyle = new GUIStyle();
	public GUIStyle glossAreaStyle = new GUIStyle();
	public GUIStyle buttonGlossStyle = new GUIStyle();

	public static MenuGUI instance;
	
	void Awake ()
	{
		instance = this;
		enabled = false;
	}

	void Update()
	{
		bool menuButtonPressed = Input.GetButtonDown("Menu");
		if ( menuButtonPressed )
		{
			enabled = false;
		}
	}

	// Update is called once per frame
	void OnGUI() 
	{
		Rect area = new Rect( Screen.width / 2 - 226, Screen.height / 2 - 119, 451, 239 );
		doGUILayer(area, areaStyle, true);
		doGUILayer(area, glossAreaStyle, false);
	}

	void doGUILayer( Rect area, GUIStyle areaStyle, bool background )
	{
		GUILayout.BeginArea( area, areaStyle );
		GUILayout.BeginVertical( GUILayout.ExpandHeight(true) );
		for ( int y = 0; y < 2; y++)
		{
			GUILayout.BeginHorizontal();
			for (int x = 0; x <= 4; x++ )
			{
				if ( background )
				{
					int sceneNr = ( x + y * 5 );
					if ( sceneNr < Application.levelCount )
					{
						if (GUILayout.Button( "" + ( sceneNr + 1 ), buttonStyle ))
						{
							Application.LoadLevel(sceneNr);
						}
					} else
					{
						GUILayout.Box( "" + ( sceneNr + 1 ), buttonDisabledStyle );
					}
				}
				else
				{
					GUILayout.Box( "", buttonGlossStyle );
				}
				if ( x < 5 )
				{
					GUILayout.FlexibleSpace();
				}
			}
			GUILayout.EndHorizontal();
			if ( y < 1 )
			{
				GUILayout.FlexibleSpace();
			}
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
