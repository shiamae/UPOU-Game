using UnityEngine;

public class TrashCan : MonoBehaviour
{
    [Header("Accepted Waste")]
    public WasteCategory acceptedCategory;

    [Header("Scoring")]
    public int correctPoints = 10;
    public int wrongPoints = -5;

    private void OnTriggerEnter(Collider other)
    {
        PickupObject pickup = other.GetComponent<PickupObject>();

        // Ignore objects that aren't being held
        if (pickup == null || !pickup.IsHeld)
            return;

        WasteItem waste = other.GetComponent<WasteItem>();

        if (waste == null)
            return;

        // Correct bin
        if (waste.category == acceptedCategory)
        {
            Debug.Log($"+{correctPoints} points! Correctly disposed of {waste.itemName}.");

            pickup.Dispose();
        }
        // Wrong bin
        else
        {
            Debug.Log($"{wrongPoints} points! Wrong trash can for {waste.itemName}.");

            // Don't destroy it.
            // The player is still holding the object and can try another bin.
        }
    }
}