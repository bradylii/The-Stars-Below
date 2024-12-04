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
    private Collider headCollider;
    private Collider camCollider;
    private bool isFlyingUp = false;  // Flag to track if the movement has started

    void Start()
    {
        portalManager = GetComponent<PortalManager>();
        starInteraction = GetComponent<StarInteraction>();
        headCollider = head.GetComponent<Collider>();

        // Get the Rigidbody component from the head object
        headRb = head.GetComponent<Rigidbody>();
        camCollider = realWorldLocation.GetComponent<Collider>();
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
        if (portalManager.isVR)
        {
            // Disable kinematic component in VR mode (use physics)
            headRb.isKinematic = false;
            headRb.useGravity = true;
            camCollider.enabled = true;

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
            float halfWidthPortal = portalFloor.transform.localScale.x / 2f;
            float halfLengthPortal = portalFloor.transform.localScale.z / 2f;

            // Check if the head is within the bounds of the portal floor (2D check)
            isWithinPortalFloor =
                realWorldLocation.transform.position.x >= (portalCenter.x - halfWidthPortal) &&
                realWorldLocation.transform.position.x <= (portalCenter.x + halfWidthPortal) &&
                realWorldLocation.transform.position.z >= (portalCenter.z - halfLengthPortal) &&
                realWorldLocation.transform.position.z <= (portalCenter.z + halfLengthPortal) &&
                head.transform.position.y > -13.8f;

            if (isWithinPortalFloor)
            {
                Debug.Log("withinPortal");
            }

            if (isWithinVirtualFloor)
            {
                Debug.Log("withinVirtualFLOOR");
            }


            // if (starCarryBack)
            // {
            //     if (targetPosition == Vector3.zero) // Check if the targetPosition has not been set yet
            //     {
            //         // Create a target position based on the real-world location's X and Z, and set Y to -13.8
            //         targetPosition = new Vector3(
            //             realWorldLocation.transform.position.x, // Use the X position from realWorldLocation
            //             -13.8f,                                 // Keep Y fixed at -13.8
            //             realWorldLocation.transform.position.z  // Use the Z position from realWorldLocation
            //         );
            //     }

            //     // Smoothly move the head towards the target position
            //     head.transform.position = Vector3.Lerp(
            //         head.transform.position,       // Start position (current head position)
            //         targetPosition,                // End position (real world location with fixed Y)
            //         carryBackSpeed * Time.deltaTime // Smooth movement speed
            //     );

            //     // Optionally, stop the movement once close enough to the target
            //     if (Vector3.Distance(new Vector3(head.transform.position.x, -13.8f, head.transform.position.z), targetPosition) < 0.2f)
            //     {
            //         // Snap to the exact target position when close enough
            //         head.transform.position = targetPosition;

            //         // Reset the carry-back flag after reaching the target
            //         starCarryBack = false;
            //         targetPosition = Vector3.zero; // Reset target position
            //     }
            // }

            // Inside the virtual floor area, disable flying and jump
            if (isWithinVirtualFloor || isWithinPortalFloor) // jumping, colliders off 
            {
                headRb.useGravity = true;


                if (isWithinPortalFloor && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
                {
                    Debug.Log("flyintopass");

                    float targetYPosition = 0f;

                    // Start the flying/jumping process
                    if (head.transform.position.y < targetYPosition && !isFlyingUp)
                    {
                        Debug.Log("doingthis");
                        isFlyingUp = true;  // Set the flag to true so that the head keeps moving up
                    }
                }

                // If we started flying up, continue the movement until we reach the target position
                if (isFlyingUp)
                {
                    float targetYPosition = 0f;

                    // Move the head up gradually towards y = 0 using Lerp or MoveTowards
                    head.transform.position = Vector3.Lerp(
                        head.transform.position,  // Current position
                        new Vector3(head.transform.position.x, targetYPosition, head.transform.position.z),  // Target position (y = 0)
                        Time.deltaTime * 1f  // Smooth movement speed
                    );

                    // Once the head reaches the target position, stop the movement and perform actions
                    if (Mathf.Abs(head.transform.position.y - targetYPosition) < 0.1f)
                    {
                        // Snap the head position exactly to y = 0 to avoid any floating point precision errors
                        head.transform.position = new Vector3(head.transform.position.x, targetYPosition, head.transform.position.z);

                        // Disable the collider and enable AR mode
                        camCollider.enabled = false;
                        portalManager.EnableARMode();

                        isFlyingUp = false;  // Reset the flag once the movement is complete
                    }
                }
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
                {
                    Debug.Log("jump");
                    // JUMP: Apply an upward force
                    headRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                }

            }
            else if (!isWithinPortalFloor && !isWithinVirtualFloor) // flying, turn on colliders, 
            {
                //Flying mode
                if (!isWithinVirtualFloor && !isWithinPortalFloor)
                {
                    headRb.useGravity = false;
                }

                //if (head.transform.position.y > -9f)
                //{
                //    headRb.useGravity = true;
                //}
                //else
                //{
                //    
                //}

                // Check if the trigger is held down to start flying
                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
                {
                    isFlying = true; // Enable flying mode
                    headRb.useGravity = false; // Disable gravity during flight

                    // Get the controller's forward direction
                    Vector3 controllerDirection = controller.transform.forward;


                    // Apply force to move in the controller's direction
                    headRb.AddForce(controllerDirection * speed, ForceMode.Acceleration);
                }
                else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
                {
                    isFlying = false; // Disable flying mode
                                      //headRb.useGravity = false;


                }
            }
        }
        else
        {
            // If not in VR mode, enable kinematic mode
            headRb.isKinematic = true;
            headRb.useGravity = true; // Enable gravity
        }
    }
}
