using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float interactionDistance = 10f;
    public Transform holdPoint;

    private PickupObject heldObject;
    private WasteInfoUI wasteInfoUI;

    private void Start()
    {
        wasteInfoUI = FindAnyObjectByType<WasteInfoUI>();
    }

    void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (!Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            Debug.Log("Clicked on nothing.");
            return;
        }

        // ===============================
        // If holding something
        // ===============================
        if (heldObject != null)
        {
            TrashCan trashCan = hit.collider.GetComponent<TrashCan>();

            if (trashCan != null)
            {
                trashCan.TryDispose(heldObject);

                // If the object was disposed, clear the reference and hide the UI
                if (heldObject != null && heldObject.IsDisposed)
                {
                    heldObject = null;

                    if (wasteInfoUI != null)
                    {
                        wasteInfoUI.HideInfo();
                    }
                }

                return;
            }

            // Click anywhere else = drop
            heldObject.Release();

            if (wasteInfoUI != null)
                wasteInfoUI.HideInfo();

            heldObject = null;

            return;
        }

        // ===============================
        // Pick up an object
        // ===============================
        PickupObject pickup = hit.collider.GetComponent<PickupObject>();

        if (pickup != null)
        {
            pickup.PickUp(holdPoint);

            heldObject = pickup;

            WasteItem waste = pickup.GetComponent<WasteItem>();

            if (waste != null && wasteInfoUI != null)
            {
                wasteInfoUI.ShowInfo(waste);
            }

            Debug.Log("Picked up " + pickup.name);
        }
    }
}