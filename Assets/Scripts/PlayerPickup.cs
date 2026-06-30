using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    public float pickupDistance = 10f;
    public Transform holdPoint;

    private PickupObject heldObject;

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E key pressed!");

            // Drop object if already holding one
            if (heldObject != null)
            {
                Debug.Log("Dropping " + heldObject.name);

                heldObject.Drop();
                heldObject = null;
                return;
            }

            // Try to pick up an object
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
            {
                Debug.Log("Hit: " + hit.collider.name);

                PickupObject pickup = hit.collider.GetComponent<PickupObject>();

                if (pickup != null)
                {
                    Debug.Log("Picked up: " + pickup.name);

                    pickup.PickUp(holdPoint);
                    heldObject = pickup;
                }
                else
                {
                    Debug.Log("Object has no PickupObject script.");
                }
            }
            else
            {
                Debug.Log("Raycast hit nothing.");
            }
        }
    }
}