using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarTreeMatching : MonoBehaviour
{
    public string requiredStarTag = "Star";  // Tag for the star to check
    public string requiredHolderTag = "StarHolder";  // Tag for the star holder
    private bool isStarLocked = false;  // Flag to check if the star is locked into the holder

    // This function will be called when another collider enters this trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the correct star with the expected tag
        if (other.CompareTag(requiredStarTag))
        {
            // Check if the current object is a Star Holder (optional if needed)
            if (gameObject.CompareTag(requiredHolderTag))
            {
                // Lock the star into the holder by making it a child of the holder
                LockStarInHolder(other.gameObject);
            }
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

            // Optionally disable any further interaction (e.g., grabbing) or add other effects
            Debug.Log("Star locked into the holder!");
        }
    }
}
