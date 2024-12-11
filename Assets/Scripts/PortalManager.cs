using UnityEngine;


public class PortalManager : MonoBehaviour
{
    public Camera mainCamera; // Assign your main camera
    public GameObject head;
    public OVRPassthroughLayer passthroughLayer; // Assign your Passthrough Layer
    public GameObject arFloorObject; // Assign the floor object in AR space (parent object)

    public GameObject vrFloorObject;
    public GameObject tree;
    public GameObject portal;

    public GameObject portalVRCover;

    public GameObject portalMask; //portal floor mask
    public GameObject portalStarKey;

    public bool hidePortal; // cover portal or not

    public bool isVR;

    public StarTreeMatching unlock;
    public GameObject rockFall;



    // Enable AR Mode
    public void EnableARMode()
    {
        head.transform.position = new Vector3(0, 0, 0);
        isVR = false;

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

        if (vrFloorObject != null)
        {
            ToggleChildMeshRenderers(vrFloorObject, true); // Enable all child MeshRenderers
        }
        tree.SetActive(true);

        portal.SetActive(false);
        portalVRCover.SetActive(false);

        portalStarKey.transform.position = new Vector3(-0.292f, 0.6f, -2.01f);

        portalMask.SetActive(true);

        hidePortal = true;

        unlock.resetKey();

    }

    // Enable VR Mode
    public void EnableVRMode()
    {
        isVR = true;
        head.transform.position += new Vector3(0f, -1.5f, 0f);

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

        if (vrFloorObject != null)
        {
            ToggleChildMeshRenderers(vrFloorObject, false); // Disable all child MeshRenderers
        }
        tree.SetActive(false);

        portalVRCover.SetActive(true);
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
        //unlock = GetComponent<StarTreeMatching>();
        EnableARMode();
    }

    void Update()
    {
        Vector3 portalCenter = portalMask.transform.position;
        float halfWidthPortal = portalMask.transform.localScale.x / 2f;
        float halfLengthPortal = portalMask.transform.localScale.z / 2f;

        // Define a buffer zone (a margin where the user has to be deeper within the portal region)
        float portalEdgeBuffer = 0.5f; // Adjust this value to determine how deep the user needs to be inside the portal region

        // Check if the head is within the inner bounds of the portal floor (2D check)
        bool isWithinPortalFloor =
            mainCamera.transform.position.x > (portalCenter.x - halfWidthPortal + portalEdgeBuffer) &&
            mainCamera.transform.position.x < (portalCenter.x + halfWidthPortal - portalEdgeBuffer) &&
            mainCamera.transform.position.z > (portalCenter.z - halfLengthPortal + portalEdgeBuffer) &&
            mainCamera.transform.position.z < (portalCenter.z + halfLengthPortal - portalEdgeBuffer);

        if (isWithinPortalFloor && !hidePortal && !isVR)
        {
            EnableVRMode();
            rockFall.SetActive(false);

        }

        if (StarTreeMatching.unlockedPortal)
        {
            portalMask.SetActive(false);
            hidePortal = false;
            rockFall.SetActive(false);
        }


        //if (head.transform.position.y < 0)
        //{
        //    EnableVRMode();
        //}
        //else if (head.transform.position.y > 0)
        //{
        //    EnableARMode();
        //}


    }

}