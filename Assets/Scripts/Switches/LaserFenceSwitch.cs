using UnityEngine;
using System.Collections;

public class LaserFenceSwitch : SwitchUnitTarget
{
	private GameObject lasers;	
	private Color currentColor;
	
	public void Awake()
	{
		enabled = false;
		lasers = transform.Find("fx_laserFence_lasers").gameObject;
		currentColor = lasers.GetComponent<Renderer>().material.color;
	}
	
	public void Update()
	{
		currentColor.a -= Time.deltaTime;
		lasers.GetComponent<Renderer>().material.color = currentColor;
		
		if ( currentColor.a <= 0.001 )
		{
			enabled = false;
			lasers.SetActive(false);
			if ( switchingDone != null )
			{
				switchingDone();
			}
		}
	}
	
	public override void Switch()
	{
		didSwitch = true;
		enabled = true;
		GetComponent<BoxCollider>().enabled = false;
	}
}

