using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRMovement : MonoBehaviour
{
    public GameObject head; // Reference to the head object
    public GameObject realWorldLocation; // Destination for carryPlayerBack
    public GameObject controller; // Reference to the controller object
    public GameObject virtualFloor; // Reference to the virtual floor
    public GameObject portalFloor; // Reference to location of portal floor (fallen floor, opening up the real world)

    public float speed = 5f; // Flying speed
    public float gravity = 9.8f; // Simulated gravity
    public float upwardForce = 1f; // Upward force when flying
    public float jumpForce = 7f; // Jump force when jumping
    public float carryBackSpeed = 2f; // Speed for carrying the player back

    private Rigidbody headRb; // Rigidbody on the head object
    private bool isFlying = false; // Whether the user is in flying mode
    private bool isCarryingPlayer = false; // Whether the player is being carried back
    public bool starCarryBack = false; // Updated in star interaction script (on star objects)

    private Vector3 carryStartPosition; // Starting position for carryPlayerBack
    private float carryProgress = 0f; // Progress for smooth carry back

    private bool isWithinVirtualFloor;
    private bool isWithinPortalFloor;

    void Start()
    {
        // Get the Rigidbody component from the head object
        headRb = head.GetComponent<Rigidbody>();
        if (headRb == null)
        {
            Debug.LogError("Rigidbody not found on the head object!");
            return;
        }

        // Ensure Rigidbody settings are suitable for flying
        headRb.useGravity = true; // Enable gravity initially
        headRb.drag = 0.5f; // Simulates air resistance
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            starCarryBack = true;
        }
        // Calculate the bounds of the virtual floor
        Vector3 floorCenter = virtualFloor.transform.position;
        float halfWidth = virtualFloor.transform.localScale.x / 2f;
        float halfLength = virtualFloor.transform.localScale.z / 2f;

        // Check if the head is within the bounds of the virtual floor (2D check)
        isWithinVirtualFloor =
            realWorldLocation.transform.position.x >= (floorCenter.x - halfWidth) &&
            realWorldLocation.transform.position.x <= (floorCenter.x + halfWidth) &&
            realWorldLocation.transform.position.z >= (floorCenter.z - halfLength) &&
            realWorldLocation.transform.position.z <= (floorCenter.z + halfLength);

        Vector3 portalCenter = portalFloor.transform.position;
        float halfWidthPortal = virtualFloor.transform.localScale.x / 2f;
        float halfLengthPortal = virtualFloor.transform.localScale.z / 2f;

        // Check if the head is within the bounds of the portal floor (2D check)
        isWithinPortalFloor =
            realWorldLocation.transform.position.x >= (floorCenter.x - halfWidthPortal) &&
            realWorldLocation.transform.position.x <= (floorCenter.x + halfWidthPortal) &&
            realWorldLocation.transform.position.z >= (floorCenter.z - halfLengthPortal) &&
            realWorldLocation.transform.position.z <= (floorCenter.z + halfLengthPortal);


        if (starCarryBack && !isWithinVirtualFloor)
        {
            //headRb.useGravity = false;

            // Calculate the direction from the current head position to the realWorldLocation
            Vector3 direction = (realWorldLocation.transform.position - head.transform.position).normalized;

            // Move the head towards the realWorldLocation gradually
            head.transform.position += direction * carryBackSpeed * Time.deltaTime;

            // Snap to the realWorldLocation position if close enough
            if (Vector3.Distance(head.transform.position, realWorldLocation.transform.position) < 0.1f)
            {
                head.transform.position = realWorldLocation.transform.position;
                starCarryBack = false; // Reset the carry-back flag
            }

        }

        // Inside the virtual floor area, disable flying and jump
        else if (isWithinVirtualFloor)
        {
            
            headRb.useGravity = true;

            //if (isWithinPortalFloor && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
            //{
            //    // JUMP/FLY UP INTO PASS THROUGH 
            //    // 
            //}
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
            {
                // JUMP: Apply an upward force
                headRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }

        }
        else
        {
 
            // Check if the trigger is held down to start flying
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
            {
                isFlying = true; // Enable flying mode
                headRb.useGravity = false; // Disable gravity during flight

                // Get the controller's forward direction
                Vector3 controllerDirection = controller.transform.forward;
                Debug.Log(controllerDirection);

                // Apply force to move in the controller's direction
                headRb.AddForce(controllerDirection * speed, ForceMode.Acceleration);
            }
            else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
            {
                isFlying = false; // Disable flying mode
                headRb.useGravity = false;
            }

            //// Apply upward force if flying
            //if (isFlying)
            //{
            //    headRb.AddForce(Vector3.up * upwardForce, ForceMode.Acceleration);
            //}



        }

    }



}
