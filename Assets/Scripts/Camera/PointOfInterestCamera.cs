using UnityEngine;
using System.Collections;

public class PointOfInterestCamera : MonoBehaviour
{
	public float duration = 3.0f;
    public float smooth = 1.5f;         // The relative speed at which the camera will catch up.
    public Transform target;            // Reference to the player's transform.
    
	public Vector3 relativeCameraPos;   
	public Vector3 cameraDirection;

	public delegate void TargetReachedDelegate();
	public TargetReachedDelegate targetReached;
    
	private float elapsed = 0;
	private Vector3 startPos, targetPos;
	private Quaternion startRotation, targetRotation;
       
    void Update ()
    {
		if ( elapsed == 0 )
		{
			startPos = transform.position;
			startRotation = transform.rotation;
			targetPos = target.position + target.transform.TransformDirection( relativeCameraPos );
			targetRotation = Quaternion.LookRotation( target.transform.TransformDirection( cameraDirection ), Vector3.up );
		}
		
		elapsed += Time.deltaTime;
		
		float t;
		if ( elapsed <= duration )
		{
    		t = computeCubicBezierCurveInterpolation(elapsed / duration, 0.42f, 0, 0.58f, 1);
			
			// Lerp the camera's rotation between it's current rotation and the rotation that looks at the player.
			transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
		} else
		{
			t = 1;
			enabled = false;
			elapsed = 0;
			if ( targetReached != null )
			{
				targetReached();
			}
		}
        
		transform.position = Vector3.Lerp( startPos, targetPos, t );
    }

	public void reverse()
	{
		targetPos = startPos;
		targetRotation = startRotation;
		startPos = transform.position;
		startRotation = transform.rotation;
		// Make this != 0 so it does not get reset in FixedUpdate
		elapsed = 0.0001f;
	}
	
	float computeCubicBezierCurveInterpolation( float t, float x1, float y1, float x2, float y2 ) 
	{
	    // Extract X (which is equal to time here)
	    float f0 = 1 - 3 * x2 + 3 * x1;
	    float f1 = 3 * x2 - 6 * x1;
	    float f2 = 3 * x1;
	 
	    var refinedT = t;
	    for (var i = 0; i < 5; i++) {
	        var refinedT2 = refinedT * refinedT;
	        var refinedT3 = refinedT2 * refinedT;
	 
	        var x = f0 * refinedT3 + f1 * refinedT2 + f2 * refinedT;
	        float slope = 1f / (3f * f0 * refinedT2 + 2f * f1 * refinedT + f2);
	        refinedT -= (x - t) * slope;
	        refinedT = Mathf.Min(1, Mathf.Max(0, refinedT));
	    }
	 
	    // Resolve cubic bezier for the given x
	    return 3 * Mathf.Pow(1 - refinedT, 2) * refinedT * y1 +
	            3 * (1 - refinedT) * Mathf.Pow(refinedT, 2) * y2 +
	            Mathf.Pow(refinedT, 3);
	} 

	void OnDrawGizmosSelected() 
	{
	    if (target == null)
		{
			return;
		}

		Gizmos.color = Color.magenta;
		Vector3 position = target.position + target.transform.TransformDirection( relativeCameraPos );
		Gizmos.DrawWireSphere(position, 0.1f);
		Gizmos.DrawLine(position, position + target.transform.TransformDirection( cameraDirection ) );
    }
}
