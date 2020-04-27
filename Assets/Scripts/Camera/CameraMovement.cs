using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    public float smooth = 1.5f;         // The relative speed at which the camera will catch up.
    public Transform player;            // Reference to the player's transform.
	
    public Vector3 relCameraPos;       // The relative position of the camera from the player.
    private float[] relCameraPosMag;      // The distance of the camera from the player.
    private Vector3 newPos;             // The position the camera is trying to reach.
	
	private bool inAbovePosition = false;
    
    void Awake ()
    {        
        transform.position = player.position + relCameraPos;
		relCameraPosMag = new float[4];
        relCameraPosMag[0] = relCameraPos.magnitude - 0.5f;
		relCameraPosMag[1] = Vector3.Lerp(relCameraPos, Vector3.up * relCameraPosMag[0], 0.25f).magnitude - 0.5f;
		relCameraPosMag[2] = Vector3.Lerp(relCameraPos, Vector3.up * relCameraPosMag[0], 0.5f).magnitude - 0.5f;
		relCameraPosMag[3] = Vector3.Lerp(relCameraPos, Vector3.up * relCameraPosMag[0], 0.75f).magnitude - 0.5f;	
	}
    
    void FixedUpdate ()
    {
        // The standard position of the camera is the relative position of the camera from the player.
        Vector3 standardPos = player.position + relCameraPos;
        
        // The abovePos is directly above the player at the same distance as the standard position.
        Vector3 abovePos = player.position + Vector3.up * relCameraPosMag[0];
        
        // An array of 5 points to check if the camera can see the player.
        Vector3[] checkPoints = new Vector3[4];
        
        // The first is the standard position of the camera.
        checkPoints[0] = standardPos;
        
        // The next three are 25%, 50% and 75% of the distance between the standard position and abovePos.
        checkPoints[1] = Vector3.Lerp(standardPos, abovePos, 0.25f);
        checkPoints[2] = Vector3.Lerp(standardPos, abovePos, 0.5f);
        checkPoints[3] = Vector3.Lerp(standardPos, abovePos, 0.75f);
        
        // The last is the abovePos.
        newPos = abovePos;
		inAbovePosition = true;
        
        // Run through the check points...
        for(int i = 0; i < checkPoints.Length; i++)
        {
            // ... if the camera can see the player...
            if(ViewingPosCheck(checkPoints[i], relCameraPosMag[i]))
			{
                inAbovePosition = false;
				// ... break from the loop.
                break;
			}
        }
        
        // Lerp the camera's position between it's current position and it's new position.
        transform.position = Vector3.Lerp(transform.position, newPos, smooth * Time.deltaTime);
        
        // Make sure the camera is looking at the player.
        SmoothLookAt();
    }
    
    
    bool ViewingPosCheck(Vector3 checkPos, float length)
    {
        RaycastHit hit;
        
        // If a raycast from the check position to the player hits something...
		Ray ray = new Ray(checkPos, player.position - checkPos);
		//Debug.DrawLine( ray.origin, ray.origin + length * ray.direction.normalized, Color.gray, 0.1f, true );
        if ( Physics.Raycast( ray, out hit, length ) )
            // ... if it is not the player...
            if( !hit.collider.isTrigger && hit.transform != player)
                // This position isn't appropriate.
                return false;
        
        // If we haven't hit anything or we've hit the player, this is an appropriate position.
        newPos = checkPos;
        return true;
    }
    
    void SmoothLookAt ()
    {
        // Create a vector from the camera towards the player.
        Vector3 relPlayerPosition = player.position - transform.position;
        
		// Create a rotation based on the relative position of the player being the forward vector.
        Quaternion lookAtRotation;
		
		if ( !inAbovePosition )
		{
			lookAtRotation = Quaternion.LookRotation( relPlayerPosition, Vector3.up );
		}
		else
		{
			Vector3 dir = new Vector3( -relCameraPos.x, 0, -relCameraPos.z );
			lookAtRotation = Quaternion.LookRotation(Vector3.down, dir);
		}        
        
        // Lerp the camera's rotation between it's current rotation and the rotation that looks at the player.
        transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, smooth * Time.deltaTime);
    }
}
