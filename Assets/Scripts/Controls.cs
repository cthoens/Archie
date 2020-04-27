using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour 
{
	public static bool keyPushed;
	public static bool switchButtonPressed;

	public delegate void SwitchButtonPressedDelegate();
	public static SwitchButtonPressedDelegate OnSwitchButtonPressed;
	
	internal static Vector3 desiredFacing = new Vector3( 0, 0, 1 );
	private Vector2 touchStart = Vector3.zero;
	private static Rect defaultPixelInset;
	private static GameObject button;

	internal static Controls instance;

	void Awake()
	{
		instance = this;
		//defaultPixelInset = guiTexture.pixelInset;
		button = transform.GetChild(0).gameObject;
		button.SetActive(false);
	}
	
	void Update() 
	{	
		if ( Input.touchCount>0 )
		{
			HandleTouchInput();
		} else
		{
			HandleKeyboardInput();
		}
	}
	
	private void HandleTouchInput()
	{
		Touch touch = Input.GetTouch(0);
		switch ( touch.phase )
		{
			case TouchPhase.Began: 
				HandleTouchBegan( touch );
				break;
			
			case TouchPhase.Moved:
				HandleTouchMoved( touch );
				break;
			
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				HandleTouchEnded( touch );
				break;
		}
		
		Rect buttonRect = new Rect(defaultPixelInset);
		switchButtonPressed = false;		
		buttonRect.x = Screen.width - buttonRect.width - defaultPixelInset.x;
		InflateRect( ref buttonRect, 20 );
		for ( int i = 0; i < Input.touchCount; i++ )
		{		
			if ( buttonRect.Contains( Input.GetTouch(i).position ) )
			{
				switchButtonPressed = true;
				if ( OnSwitchButtonPressed != null )
				{
					OnSwitchButtonPressed();
				}

				break;
			}
		}
	}

	private void HandleTouchBegan( Touch touch )
	{
        /*
		if ( touch.position.x > Screen.width / 2 )
		{
			return;
		}
		
		Rect controlsPos = guiTexture.pixelInset;
		InflateRect( ref controlsPos, 128 );
		if ( controlsPos.Contains( touch.position ) )
		{
			touchStart = new Vector2( guiTexture.pixelInset.x + 64, guiTexture.pixelInset.y + 64);
			HandleTouchMoved( touch );
		} else
		{
			touchStart = touch.position;
			Rect pixelInset = new Rect( touchStart.x - 64, touchStart.y - 64, 128, 128 );
			guiTexture.pixelInset = pixelInset;
		}
		keyPushed = false;
        */
	}

	float deltaX;
	float deltaY;	

	private void HandleTouchMoved( Touch touch )
	{
		if ( touchStart == Vector2.zero ) 
		{
			return;
		}
		deltaX = touch.position.y - touchStart.y;
		deltaY = touchStart.x - touch.position.x;					
		float x = 0;				
		if (deltaX < -50) x = -1;
		else if (deltaX > 50) x = 1;				
		float z = 0;
		if (deltaY < -50) z = -1;
		else if (deltaY > 50) z = 1;
		
		keyPushed = x != 0.0f || z != 0.0f;
		if ( keyPushed )
		{
			desiredFacing.x = x;
			desiredFacing.z = z;	 
		}
	}

//	void OnGUI()
//	{
//		GUI.Box(new Rect(touchStart.x, Screen.height - touchStart.y, -deltaY, -deltaX), "");
//	}

	void HandleTouchEnded( Touch touch )
	{
        /*
        if ( touchStart == Vector2.zero ) 
		{
			return;
		}

		keyPushed = false;	
		guiTexture.pixelInset = defaultPixelInset;
		touchStart = Vector3.zero;
        */
	}
	
	private void InflateRect( ref Rect rect, float amount )
	{
		rect.x -= amount;
		rect.y -= amount;
		rect.width += amount;
		rect.height += amount;
	}
	
	private void HandleKeyboardInput()
	{
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");

		keyPushed = horizontal != 0.0f || vertical != 0.0f;
		if ( keyPushed )
		{
			desiredFacing.x = horizontal;
			desiredFacing.z = vertical;
		}

		// Handle switching buttons
		switchButtonPressed = Input.GetButtonDown("Fire1");
		if ( switchButtonPressed )
		{
			if ( OnSwitchButtonPressed != null )
			{
				OnSwitchButtonPressed();
			}
		}

		bool menuButtonPressed = Input.GetButtonDown("Menu");
		if ( menuButtonPressed && !MenuGUI.instance.enabled )
		{
			MenuGUI.instance.enabled = true;
		}
	}

    public static void SetButtonVisible(bool value)
	{
		button.SetActive(value);
		if (value) 
		{
			Rect buttonRect = new Rect(defaultPixelInset);
			buttonRect.x = Screen.width - buttonRect.width - defaultPixelInset.x;
			//button.guiTexture.pixelInset = buttonRect;
		}			
	}

	public static bool TouchBegan()
	{
		for ( int i = 0; i < Input.touchCount; i++ )
		{
			if ( Input.GetTouch(i).phase == TouchPhase.Began )
			{
				return true;
			}
		}

		return false;
	}
}
