using UnityEngine;
using System.Collections;

public class AnimationTrigger : MonoBehaviour 
{
	Animator animator;

	void Awake()
	{
		animator = GetComponent<Animator>();
	}

	void OnTriggerEnter( Collider other )
	{
		if ( other.isTrigger )
		{
			return;
		}

		animator.SetBool("Triggered", true);
	}

	void OnTriggerExit( Collider other )
	{
		if ( other.isTrigger )
		{
			return;
		}

		animator.SetBool("Triggered", false);
	}
}
