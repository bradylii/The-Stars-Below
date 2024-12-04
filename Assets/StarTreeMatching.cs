using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarTreeMatching : MonoBehaviour
{
    public string requiredStarTag;  // Tag for the star to check
    public string requiredHolderTag;  // Tag for the star holder
    private bool isStarLocked = false;  // Flag to check if the star is locked into the holder
    public Color lockedColor = Color.green;  // Color to change the holder and portalOpener to when the star is locked

    public static bool unlockedPortal = false;

    public GameObject portalOpener;  // Reference to the portalOpener GameObject

    // This function will be called when another collider enters this trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the correct star with the expected tag
        if (other.CompareTag(requiredStarTag))
        {
            Debug.Log("connect");
            // Lock the star into the holder by making it a child of the holder
            LockStarInHolder(other.gameObject);

            unlockedPortal = true;
        }
    }

    public void resetKey()
    {
        // Reset the color of the holder and portalOpener back to white (or any color that represents the "unlocked" state)
        //ChangeHolderColor(Color.white);
        ChangePortalOpenerColor(Color.white);

        // Set the unlockedPortal flag back to false
        unlockedPortal = false;

        Debug.Log("Key reset: Portal is now locked and objects are back to original state.");
    }

    // Lock the star into the holder
    private void LockStarInHolder(GameObject star)
    {
        // Save the original scale of the star
        Vector3 originalScale = star.transform.localScale;

        // Move the star to the position of the holder
        star.transform.position = this.transform.position;

        // Make the star a child of the parent object’s parent (grandparent)
        Transform parentTransform = this.transform.parent; // Get the parent of the current object (grandparent)
        star.transform.SetParent(parentTransform);

        // Adjust the scale of the star based on the parent's scale (if needed)
        Vector3 parentScale = this.transform.localScale;
        star.transform.localScale = new Vector3(
            originalScale.x / parentScale.x,
            originalScale.y / parentScale.y,
            originalScale.z / parentScale.z
        );

        // Change the color of the star holder and portalOpener to green
        ChangeHolderColor(lockedColor);
        ChangePortalOpenerColor(lockedColor);

        // Optionally disable any further interaction (e.g., grabbing) or add other effects
        Debug.Log("Star locked into the holder!");
    }



    // Change the color of the holder object to the specified color
    private void ChangeHolderColor(Color newColor)
    {
        // Create a new green material
        Material greenMaterial = new Material(Shader.Find("Standard"));  // You can use any shader you prefer
        greenMaterial.color = newColor;  // Set the material color to green or the specified color

        // Get the Renderer component of the holder
        Renderer holderRenderer = GetComponent<Renderer>();

        if (holderRenderer != null)
        {
            // Apply the new material to the object
            holderRenderer.material = greenMaterial;
        }
        else
        {
            Debug.LogWarning("Renderer not found on the holder object!");
        }
    }


    // Change the color of the portalOpener object to the specified color
    private void ChangePortalOpenerColor(Color newColor)
    {
        if (portalOpener != null)
        {
            Renderer portalOpenerRenderer = portalOpener.GetComponent<Renderer>();  // Get the Renderer component of portalOpener

            if (portalOpenerRenderer != null)
            {
                portalOpenerRenderer.material.color = newColor;  // Change the material color
            }
            else
            {
                Debug.LogWarning("Renderer not found on the portalOpener object!");
            }
        }
        else
        {
            Debug.LogWarning("PortalOpener GameObject is not assigned!");
        }
    }
}
