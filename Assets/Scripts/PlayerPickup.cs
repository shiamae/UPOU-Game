using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupDistance = 10f;
    public Transform holdPoint;

    private PickupObject heldObject;
    private WasteInfoUI wasteInfoUI;

    private void Start()
    {
        // Find the UI manager in the scene
        wasteInfoUI = FindAnyObjectByType<WasteInfoUI>();

        if (wasteInfoUI == null)
        {
            Debug.LogWarning("No WasteInfoUI found in the scene!");
        }
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Drop the currently held object
            if (heldObject != null)
            {
                Debug.Log("Dropped: " + heldObject.name);

                heldObject.Drop();

                // Hide the information panel
                if (wasteInfoUI != null)
                {
                    wasteInfoUI.HideInfo();
                }

                heldObject = null;
                return;
            }

            // Raycast from the mouse cursor
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            Debug.DrawRay(ray.origin, ray.direction * pickupDistance, Color.red, 2f);

            if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
            {
                Debug.Log("Clicked: " + hit.collider.name);

                PickupObject pickup = hit.collider.GetComponent<PickupObject>();

                if (pickup != null)
                {
                    Debug.Log("Picked up: " + pickup.name);

                    pickup.PickUp(holdPoint);
                    heldObject = pickup;

                    // Display waste information
                    WasteItem wasteItem = pickup.GetComponent<WasteItem>();

                    if (wasteItem != null)
                    {
                        if (wasteInfoUI != null)
                        {
                            wasteInfoUI.ShowInfo(wasteItem);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("This pickup object has no WasteItem component.");
                    }
                }
                else
                {
                    Debug.Log("This object is not pickable.");
                }
            }
            else
            {
                Debug.Log("Clicked on nothing.");
            }
        }
    }
}