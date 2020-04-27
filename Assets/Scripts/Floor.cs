using UnityEngine;
using System.Collections;

public class Floor : MonoBehaviour
{
	public float y;
	internal bool showing = false;

	public void Enter()
	{
		gameObject.SetActive(true);
		showing = true;
	}

	public void Exit()
	{
		gameObject.SetActive(false);
		showing = false;
	}
}