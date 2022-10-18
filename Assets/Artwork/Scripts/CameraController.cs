using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform followPlayer; // player
    public Vector3 offset = new Vector3(0, 6.0f, -7.0f);
    public Vector3 rotation = new Vector3(35, 0,0);   

    public bool isMoving { get; set; }
                             
   
    // LateUpdate is called after the player has moved
    void LateUpdate()
    {
        if (!isMoving)
        {
            return;
        }

        // where the player should be
        Vector3 desiredPosition = followPlayer.position + offset;
        desiredPosition.x = 0;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), 0.1f);
    }
}
