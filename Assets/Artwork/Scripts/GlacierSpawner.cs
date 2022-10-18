using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlacierSpawner : MonoBehaviour
{
    private const float DISTANCE_TO_RESPAWN = 10.0f;

    public float scrollSpeed = -2;
    public float totalLength;
    public bool isScrolling { get; set; }

    private float scrollLocation;
    private Transform playerTransform;

    private void Start()
    {
        // know where thr player is on the z axis
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (!isScrolling)
        {
            return; // do not do anything
        }
        // get the scroll speed on every frame
        scrollLocation += scrollSpeed * Time.deltaTime;

        //get the vector 3 location of the player
        Vector3 newLocation = (playerTransform.position.z + scrollLocation) * Vector3.forward;
        transform.position = newLocation;


        if (transform.GetChild(0).transform.position.z < playerTransform.position.z - DISTANCE_TO_RESPAWN)
        {
            transform.GetChild(0).localPosition += Vector3.forward * totalLength;
            transform.GetChild(0).SetSiblingIndex(transform.childCount);
            transform.GetChild(0).localPosition += Vector3.forward * totalLength;
            transform.GetChild(0).SetSiblingIndex(transform.childCount);
        }
    }
}
