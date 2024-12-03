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
    public float carryBackSpeed = .5f; // Speed for carrying the player back

    private Rigidbody headRb; // Rigidbody on the head object
    private bool isFlying = false; // Whether the user is in flying mode
    private bool isCarryingPlayer = false; // Whether the player is being carried back
    public bool starCarryBack = false; // Updated in star interaction script (on star objects)

    private Vector3 carryStartPosition; // Starting position for carryPlayerBack
    private float carryProgress = 0f; // Progress for smooth carry back

    private bool isWithinVirtualFloor;
    private bool isWithinPortalFloor;

    private PortalManager portalManager;
    private StarInteraction starInteraction;

    private Vector3 targetPosition;


    void Start()
    {

        portalManager = GetComponent<PortalManager>();
        starInteraction = GetComponent<StarInteraction>();


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
            realWorldLocation.transform.position.z <= (floorCenter.z + halfLength) &&
            head.transform.position.y > -13.8f;

        Vector3 portalCenter = portalFloor.transform.position;
        float halfWidthPortal = virtualFloor.transform.localScale.x / 2f;
        float halfLengthPortal = virtualFloor.transform.localScale.z / 2f;

        // Check if the head is within the bounds of the portal floor (2D check)
        isWithinPortalFloor =
            realWorldLocation.transform.position.x >= (portalCenter.x - halfWidthPortal) &&
            realWorldLocation.transform.position.x <= (portalCenter.x + halfWidthPortal) &&
            realWorldLocation.transform.position.z >= (portalCenter.z - halfLengthPortal) &&
            realWorldLocation.transform.position.z <= (portalCenter.z + halfLengthPortal) &&
            head.transform.position.y > -13.8f;


        if (portalManager.isVR)
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))//(StarInteraction.isGrabbed && !isWithinVirtualFloor) // if carrying star, bring back to zone
            {
                starCarryBack = true;
            }
            if (starCarryBack)
            {
                if (targetPosition == Vector3.zero) // Check if the targetPosition has not been set yet
                {
                    // Create a target position based on the real-world location's X and Z, and set Y to -13.8
                    targetPosition = new Vector3(
                        realWorldLocation.transform.position.x, // Use the X position from realWorldLocation
                        -13.8f,                                 // Keep Y fixed at -13.8
                        realWorldLocation.transform.position.z  // Use the Z position from realWorldLocation
                    );
                }

                // Smoothly move the head towards the target position
                head.transform.position = Vector3.Lerp(
                    head.transform.position,       // Start position (current head position)
                    targetPosition,                // End position (real world location with fixed Y)
                    carryBackSpeed * Time.deltaTime // Smooth movement speed
                );
                //Debug.Log("Moving towards target: " + targetPosition);

                // Optionally, stop the movement once close enough to the target
                if (Vector3.Distance(new Vector3(head.transform.position.x, -13.8f, head.transform.position.z), targetPosition) < 0.2f)
                {
                    // Snap to the exact target position when close enough
                    head.transform.position = targetPosition;

                    // Reset the carry-back flag after reaching the target
                    starCarryBack = false;
                    targetPosition = Vector3.zero; // Reset target position
                }
            }


            // Inside the virtual floor area, disable flying and jump
            else if (isWithinVirtualFloor) //jumping, colliders off 
            {

                headRb.useGravity = true;

                if (isWithinPortalFloor && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
                {
                    float targetYPosition = 0f;
                    // JUMP/FLY UP INTO PASS THROUGH 
                    // Start moving the head up to y = 0 if the head is not already at y = 0
                    if (head.transform.position.y > targetYPosition)
                    {
                        // Move the head up gradually towards y = 0 using Lerp or MoveTowards
                        head.transform.position = Vector3.Lerp(
                            head.transform.position,  // Current position
                            new Vector3(head.transform.position.x, targetYPosition, head.transform.position.z),  // Target position (y = 0)
                            Time.deltaTime * 1f  // Smooth movement speed
                        );
                    }
                    else
                    {
                        // Ensure that the head stays exactly at y = 0 once it reaches that point
                        head.transform.position = new Vector3(head.transform.position.x, targetYPosition, head.transform.position.z);

                        // Enable AR Mode or other actions after the movement is complete
                        portalManager.EnableARMode();
                    }

                }
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
                {
                    // JUMP: Apply an upward force
                    headRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                }

            }
            else // flying, turn on colliders, 
            {
                //Debug.Log(targetPosition);
                if (!isWithinVirtualFloor)
                {
                    headRb.useGravity = false;
                }


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


            }
        }
        else
        {
            headRb.useGravity = true;
        }



    }



}
