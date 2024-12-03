using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarTreeMatching : MonoBehaviour
{
    public string requiredStarTag;  // Tag for the star to check
    public string requiredHolderTag;  // Tag for the star holder
    private bool isStarLocked = false;  // Flag to check if the star is locked into the holder
    public Color lockedColor = Color.green;  // Color to change the holder to when the star is locked

    // This function will be called when another collider enters this trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the correct star with the expected tag
        if (other.CompareTag(requiredStarTag))
        {
            Debug.Log("connect");
            // Lock the star into the holder by making it a child of the holder
            LockStarInHolder(other.gameObject);

        }
    }

    // Lock the star into the holder
    private void LockStarInHolder(GameObject star)
    {
        if (!isStarLocked)  // Ensure the star is not already locked
        {
            // Make the star a child of this holder object
            star.transform.SetParent(transform);

            // Optionally, reset the star's position to the holder's position (if needed)
            star.transform.localPosition = Vector3.zero;  // Align the star inside the holder at the holder's position

            // Mark the star as locked
            isStarLocked = true;

            // Change the color of the star holder to green
            ChangeHolderColor(lockedColor);

            // Optionally disable any further interaction (e.g., grabbing) or add other effects
            Debug.Log("Star locked into the holder!");
        }
    }

    // Change the color of the holder object to the specified color
    private void ChangeHolderColor(Color newColor)
    {
        Renderer holderRenderer = GetComponent<Renderer>();  // Get the Renderer component of the holder

        if (holderRenderer != null)
        {
            holderRenderer.material.color = newColor;  // Change the material color
        }
        else
        {
            Debug.LogWarning("Renderer not found on the holder object!");
        }
    }
}
