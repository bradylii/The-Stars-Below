using UnityEngine;


public class PortalManager : MonoBehaviour
{
    public Camera mainCamera; // Assign your main camera
    public GameObject head;
    public OVRPassthroughLayer passthroughLayer; // Assign your Passthrough Layer
    public GameObject arFloorObject; // Assign the floor object in AR space (parent object)
    public GameObject tree;
    public GameObject portal;

    // Enable AR Mode
    public void EnableARMode()
    {
        if (passthroughLayer != null)
        {
            passthroughLayer.hidden = false; // Show passthrough
        }

        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = new Color(0, 0, 0, 0); // Transparent background
        }

        if (arFloorObject != null)
        {
            ToggleChildMeshRenderers(arFloorObject, true); // Enable all child MeshRenderers
        }
        tree.SetActive(true);

        portal.SetActive(false); 
    }

    // Enable VR Mode
    public void EnableVRMode()
    {
        if (passthroughLayer != null)
        {
            passthroughLayer.hidden = true; // Hide passthrough
        }

        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.Skybox; // Set camera background to skybox
        }

        if (arFloorObject != null)
        {
            ToggleChildMeshRenderers(arFloorObject, false); // Disable all child MeshRenderers
        }
        tree.SetActive(false);

        portal.SetActive(true);
    }

    // Helper method to toggle child MeshRenderers
    private void ToggleChildMeshRenderers(GameObject parentObject, bool isEnabled)
    {
        var meshRenderers = parentObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = isEnabled;
        }
    }

    private void Start()
    {
        EnableARMode();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) // Switch to AR
        {
            EnableARMode();
        }
        else if (Input.GetKeyDown(KeyCode.V)) // Switch to VR
        {
            EnableVRMode();
        }

        if (head.transform.position.y < 0)
        {
            EnableVRMode();
        }
        else if (head.transform.position.y > 0)
        {
            EnableARMode();
        }

        
    }

}