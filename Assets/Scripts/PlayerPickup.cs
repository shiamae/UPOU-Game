using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    public float pickupDistance = 10f;
    public Transform holdPoint;

    private PickupObject heldObject;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // If already holding an object, drop it
            if (heldObject != null)
            {
                Debug.Log("Dropped: " + heldObject.name);

                heldObject.Drop();
                heldObject = null;
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
            {
                Debug.Log("Clicked: " + hit.collider.name);

                PickupObject pickup = hit.collider.GetComponent<PickupObject>();

                if (pickup != null)
                {
                    Debug.Log("Picked up: " + pickup.name);

                    pickup.PickUp(holdPoint);
                    heldObject = pickup;
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