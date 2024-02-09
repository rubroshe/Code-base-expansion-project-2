using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Vector3 cameraOffset = new Vector3(0,10,-3);
    public GameObject player;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Camera.main.gameObject.transform.position = player.transform.position + cameraOffset;
    }
}
