using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;  // For OVRGrabber, assumes Meta SDK for OpenXR integration

public class StarInteraction : MonoBehaviour
{
    public static bool isGrabbed = false;  // Flag to indicate if the star is grabbed
    public OVRGrabber grabber;  // Reference to the OVRGrabber component (drag this in the inspector)

    private void Start()
    {
        if (grabber == null)
        {
            Debug.LogError("OVRGrabber component not assigned in the inspector. Please assign the OVRGrabber from the controller.");
        }
    }

    private void Update()
    {
        // Checking if the controller grip button is pressed using OVRInput
        bool rightGripPressed = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);
        bool leftGripPressed = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch);

        // Detect when the Right Controller Grip button is pressed and grabbed object is detected
        if (rightGripPressed && grabber.grabbedObject != null)
        {
            isGrabbed = true;
            Debug.Log("Grabbing with Right Controller: " + grabber.grabbedObject.name);
        }
        // Detect when the Right Controller Grip button is released and check if the object is released
        else if (!rightGripPressed && grabber.grabbedObject == null)
        {
            isGrabbed = false;
            Debug.Log("Released with Right Controller");
        }

        // Detect when the Left Controller Grip button is pressed and grabbed object is detected
        if (leftGripPressed && grabber.grabbedObject != null)
        {
            isGrabbed = true;
            Debug.Log("Grabbing with Left Controller: " + grabber.grabbedObject.name);
        }
        // Detect when the Left Controller Grip button is released and check if the object is released
        else if (!leftGripPressed && grabber.grabbedObject == null)
        {
            isGrabbed = false;
            Debug.Log("Released with Left Controller");
        }
    }
}
