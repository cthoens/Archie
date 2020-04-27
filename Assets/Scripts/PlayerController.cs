using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public float angularSpeedDampTime = 0.7f;       // Damping time for the AngularSpeed parameter
	public float angleResponseTime = 0.6f;  
	public float deadZone = 5f;             // The number of degrees for which the rotation isn't controlled by Mecanim.
	public float speed = 0;
	public float maxSpeed = 2;
	public float acceleration = .25f;
	public float deceleration = 1f;
	public Collider floorCollider;
	public Floor[] floors;

	private Animator anim;
	private bool isGrounded = true;	
	public float groundedTime = 0;
	internal bool stopping = false;
	private bool isPathClear = true;
	private bool isPathClearWasUpdated = false;

	private bool isClimbing = false;
	private TriggerHelper antiClimbTrigger;

	// Use this for initialization
	void Awake() 
	{
		anim = GetComponent<Animator>();
		antiClimbTrigger = GetComponentInChildren<TriggerHelper>();

		// We need to convert the angle for the deadzone from degrees to radians.
		deadZone *= Mathf.Deg2Rad;
	}

	void Start()
	{
		Controls.desiredFacing = transform.forward;
	}

	void FixedUpdate()
	{
		if ( !isGrounded && groundedTime > 0.5f && speed >= 3 )
		{
			GetComponent<Rigidbody>().velocity += 4.0f * Vector3.up * - Physics.gravity.y / ( 2.0f * maxSpeed );
		}
	
		// ... and the angle is the angle between forward and the desired velocity.
		float angle = FindAngle(transform.forward, Controls.desiredFacing, transform.up);

		// If the angle is within the deadZone...
		if( Mathf.Abs(angle) < deadZone )
		{
			// ... set the direction to be along the desired direction and set the angle to be zero.
			transform.LookAt(transform.position + Controls.desiredFacing);
			angle = 0f;
		}	
		// Call the Setup function of the helper class with the given parameters.
		Setup(angle);

		if (stopping)
		{
			speed = 0;
			anim.SetFloat("speed", speed, 0.1f, Time.deltaTime);
			return;
		}

		if ( isGrounded )
		{
			if ( Controls.keyPushed && Mathf.Abs(angle) <= Mathf.PI / 4 + Mathf.PI / 8 )
			{
				//speed = Mathf.Min( maxSpeed, speed + acceleration );
				speed = Mathf.Lerp( speed, maxSpeed, 0.02f );
			} else {
				speed = Mathf.Max( 0, speed - deceleration );
			}
		}
		bool doClimb = false;
		if ( !isClimbing && !isPathClear )
		{
			if ( isGrounded && !antiClimbTrigger.isTriggered && Mathf.Abs(angle) < Mathf.PI / 16 && GetComponent<Rigidbody>().velocity.y >= -0.01f )
			{
				doClimb = true;
			}
			speed = 0.0f;
			anim.SetFloat("speed", speed, 0.1f, Time.deltaTime);
		} else
		{
			anim.SetFloat("speed", speed);
		}

		if ( doClimb ) 
		{
			isClimbing = true;
			anim.SetBool("climb", true);
			floorCollider.enabled = false;
		} else {
			anim.SetBool("climb", false);
		}

		// Handle floors
		foreach ( Floor f in floors )
		{
			if ( transform.position.y >= f.y && !f.showing)
			{
				f.Enter();
			}

			if ( transform.position.y < f.y && f.showing)
			{
				f.Exit();
			}
		}

		if ( isGrounded )
		{
			groundedTime += Time.deltaTime;
		} else
		{
			groundedTime = 0;
		}
		isGrounded = false;
	}

	void OnCollisionEnter( Collision other )
	{
		if ( other.collider.isTrigger )
		{
			if ( other.gameObject.tag.Equals( "DeathTrigger" ) )
			{
				isClimbing = false;
				floorCollider.enabled = true;
				anim.SetBool("dead", true);
				stopping = true;
			}
		}

		isGrounded = true;
	}

	void OnCollisionStay( Collision other )
	{
		isGrounded = true;
	}

	void OnTriggerEnter( Collider other )
	{
		if ( other.isTrigger )
		{
			if ( other.gameObject.tag.Equals( "DeathTrigger" ) )
			{
				isClimbing = false;
				floorCollider.enabled = true;
				anim.SetBool("dead", true);
				stopping = true;
			}
			return;
		}
		
		isPathClear = false;
		isPathClearWasUpdated = true;
	}

	//Note: If a GameObject that has triggered a trigger is deactivated OnTriggerExit() is
	// not called. This is why we need to rely on OnTriggerStay()
	void OnTriggerStay( Collider other )
	{
		if ( other.isTrigger )
		{
			return;
		}
		
		isPathClear = false;
		isPathClearWasUpdated = true;
	}
	
	void LateUpdate()
	{
		if ( !isPathClear && !isPathClearWasUpdated )
		{
			isPathClear = true;
		}
		isPathClearWasUpdated = false;
	}

	void OnAnimClimbingReachedTop()
	{
		isClimbing = false;
		floorCollider.enabled = true;
		speed = 1;
		anim.SetFloat("speed", speed);
	}

	void OnAnimDoneDying()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	void Setup(float angle)
	{
		// Angular speed is the number of degrees per second.
		float angularSpeed = angle / angleResponseTime;

		// Set the mecanim parameters and apply the appropriate damping to them.
		anim.SetFloat("angularSpeed", angularSpeed, angleResponseTime, Time.deltaTime);
	}   

	float FindAngle (Vector3 fromVector, Vector3 toVector, Vector3 upVector)
	{
		// If the vector the angle is being calculated to is 0...
		if(toVector == Vector3.zero)
			// ... the angle between them is 0.
			return 0f;
		
		// Create a float to store the angle between the facing of the enemy and the direction it's travelling.
		float angle = Vector3.Angle(fromVector, toVector);
		
		// Find the cross product of the two vectors (this will point up if the velocity is to the right of forward).
		Vector3 normal = Vector3.Cross(fromVector, toVector);
		
		// The dot product of the normal with the upVector will be positive if they point in the same direction.
		angle *= Mathf.Sign(Vector3.Dot(normal, upVector));
		
		// We need to convert the angle we've found from degrees to radians.
		angle *= Mathf.Deg2Rad;
		
		return angle;
	}

	void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.white;
		Gizmos.DrawLine( transform.position, transform.position + Controls.desiredFacing );
	}
}
