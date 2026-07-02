using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    public float interactionDistance = 5f;
    public Transform holdPoint;

    private PickupObject heldObject;

    private WasteInfoUI wasteInfoUI;

    void Start()
    {
        wasteInfoUI = FindAnyObjectByType<WasteInfoUI>();
    }

    void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f,.5f));

        if (!Physics.Raycast(ray,out RaycastHit hit,interactionDistance))
            return;

        //---------------------------------------------------
        // PICKUP
        //---------------------------------------------------

        if (heldObject == null)
        {
            PickupObject pickup = hit.collider.GetComponent<PickupObject>();

            if (pickup != null)
            {
                pickup.PickUp(holdPoint);

                heldObject = pickup;

                WasteItem waste = pickup.GetComponent<WasteItem>();

                if(wasteInfoUI!=null)
                    wasteInfoUI.ShowInfo(waste);

                return;
            }
        }

        //---------------------------------------------------
        // DISPOSE
        //---------------------------------------------------

        if (heldObject != null)
        {
            TrashCan trashCan = hit.collider.GetComponent<TrashCan>();

            if (trashCan != null)
            {
                WasteItem waste = heldObject.GetComponent<WasteItem>();

                if(waste.category == trashCan.acceptedCategory)
                {
                    Debug.Log("Correct Bin! +10");

                    heldObject.Dispose();

                    heldObject = null;

                    wasteInfoUI.HideInfo();
                }
                else
                {
                    Debug.Log("Wrong Bin!");

                    Debug.Log("Correct Category: " + waste.category);
                }

                return;
            }

            //---------------------------------------------------
            // DROP ON GROUND
            //---------------------------------------------------

            heldObject.Release();

            heldObject = null;

            wasteInfoUI.HideInfo();
        }
    }
}