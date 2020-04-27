using UnityEngine;
using System.Collections;

public class LightBridgeSwitch : SwitchUnitTarget
{
	float targetAlpha;
	private Color currentColor;
	
	public void Awake()
	{
		gameObject.SetActive(false);
		enabled = false;
		currentColor = GetComponent<Renderer>().material.color;
		targetAlpha = GetComponent<Renderer>().material.color.a;
		currentColor.a = 0;
		GetComponent<Renderer>().material.color = currentColor;
	}
	
	public void Update()
	{
		currentColor.a += Time.deltaTime;
		
		if ( currentColor.a >= targetAlpha )
		{
			currentColor.a = targetAlpha;
			enabled = false;
			if ( switchingDone != null )
			{
				switchingDone();
			}
		}
		
		GetComponent<Renderer>().material.color = currentColor;
	}
	
	public override bool IsSwitchingDone()
	{
		return !enabled;
	}
	
	public override void Switch()
	{
		didSwitch = true;
		enabled = true;
		gameObject.SetActive(true);
	}
}

