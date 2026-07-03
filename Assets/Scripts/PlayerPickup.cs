using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float interactionDistance = 10f;
    public Transform holdPoint;

    private PickupObject heldObject;

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

        //================================================
        // PLAYER IS ALREADY HOLDING AN OBJECT
        //================================================
        if (heldObject != null)
        {
            // Check if player clicked on a trash can
            TrashCan trashCan = hit.collider.GetComponent<TrashCan>();

            if (trashCan != null)
            {
                trashCan.TryDispose(heldObject);

                // Object was disposed
                if (heldObject != null && heldObject.IsDisposed)
                {
                    heldObject = null;
                }

                return;
            }

            // Otherwise, drop the object
            heldObject.Release();

            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.HideHint();
            }

            heldObject = null;

            return;
        }

        //================================================
        // PLAYER IS NOT HOLDING ANYTHING
        //================================================
        PickupObject pickup = hit.collider.GetComponent<PickupObject>();

        if (pickup != null)
        {
            pickup.PickUp(holdPoint);

            heldObject = pickup;

            WasteItem waste = pickup.GetComponent<WasteItem>();

            if (waste != null && GameUIManager.Instance != null)
            {
                // Show the hint popup
                GameUIManager.Instance.ShowHint(waste);
            }

            Debug.Log("Picked up " + pickup.name);
        }
    }
}