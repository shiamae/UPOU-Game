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
        // Don't allow interaction while the education popup is open
        if (Time.timeScale == 0f)
            return;

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
            TrashCan trashCan = hit.collider.GetComponent<TrashCan>();

            // Player clicked a trash can
            if (trashCan != null)
            {
                trashCan.TryDispose(heldObject);

                // If the object has been hidden/disposed,
                // stop treating it as the held object.
                if (heldObject == null ||
                    heldObject.IsDisposed ||
                    !heldObject.gameObject.activeInHierarchy)
                {
                    heldObject = null;
                }

                return;
            }

            // Player clicked somewhere else -> drop the object
            heldObject.Release();

            if (GameUIManager.Instance != null)
                GameUIManager.Instance.HideHint();

            heldObject = null;

            return;
        }

        //================================================
        // PLAYER IS NOT HOLDING ANYTHING
        //================================================
        PickupObject pickup = hit.collider.GetComponent<PickupObject>();

        if (pickup != null)
        {
            Debug.Log("Found PickupObject: " + pickup.name);

            pickup.PickUp(holdPoint);

            Debug.Log("Returned from PickUp().");

            heldObject = pickup;

            WasteItem waste = pickup.GetComponent<WasteItem>();

            if (waste != null && GameUIManager.Instance != null)
            {
                GameUIManager.Instance.ShowHint(waste);
            }

            Debug.Log("Picked up " + pickup.name);
        }
    }

    /// <summary>
    /// Called by GameUIManager after the education popup closes.
    /// </summary>
    public void ClearHeldObject()
    {
        heldObject = null;
    }
}