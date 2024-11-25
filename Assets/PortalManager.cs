using UnityEngine;


public class PortalManager : MonoBehaviour
{
    private OVRPassthroughLayer passthroughLayer;

    void Start()
    {
        // Get the Passthrough Layer component
        passthroughLayer = FindObjectOfType<OVRPassthroughLayer>();
        if (passthroughLayer == null)
        {
            Debug.LogError("OVRPassthroughLayer not found! Please add it to your scene.");
        }
    }

    void Update()
    {
        // Example: Toggle passthrough with the A button
        if (OVRInput.GetDown(OVRInput.Button.One)) // A button on Quest controllers
        {
            TogglePassthrough();
        }
    }

    void TogglePassthrough()
    {
        if (passthroughLayer != null)
        {
            passthroughLayer.enabled = !passthroughLayer.enabled;
            Debug.Log("Passthrough " + (passthroughLayer.enabled ? "enabled" : "disabled"));
        }
    }
}
