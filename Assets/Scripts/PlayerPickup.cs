using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float interactionDistance = 10f;
    public Transform holdPoint;

    private PickupObject heldObject;

    public static PlayerPickup Instance { get; private set; }

    public bool IsHoldingObject => heldObject != null;

    private float lastDisposeTime = -10f;
    public bool RecentlyDisposedTrash(float withinSeconds = 1.5f)
    {
        return Time.time - lastDisposeTime < withinSeconds;
    }

    public PickupObject HeldObject => heldObject;

    public WasteItem HeldWasteItem =>
        heldObject != null ? heldObject.GetComponent<WasteItem>() : null;

    private void Awake()
    {
        Instance = this;
    }

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
            Debug.Log("Hit: " + hit.collider.name);

            TrashCan trashCan = hit.collider.GetComponentInParent<TrashCan>();

            if (trashCan != null)
            {
                Debug.Log("Found TrashCan: " + trashCan.name);
            }
            else
            {
                Debug.Log("No TrashCan found.");
            }

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
                    lastDisposeTime = Time.time;
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