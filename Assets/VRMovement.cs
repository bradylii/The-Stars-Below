using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject head;
    public GameObject controller;
    private Vector3 controllerDirection;

    public float speed;

    private bool enableFly;
    private bool starPathBack;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if in fly region, enable, if not, jump
        //if flying, save position of where they started to fly + any ACTUAL movement user did while flying to get pathway back when they grab star 

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            Debug.Log("Pressed");
            controllerDirection = controller.transform.forward;
            head.transform.position += speed * controllerDirection; // OLD FROM CLASS: new Vector3(controllerDirection.x, 0, controllerDirection.z);
        }


    }
}
