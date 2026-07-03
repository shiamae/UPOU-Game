using UnityEngine;

public class TrashCan : MonoBehaviour
{
    [Header("Accepted Waste")]
    public WasteCategory acceptedCategory;

    [Header("Points")]
    public int correctPoints = 10;
    public int wrongPoints = -5;

    public void TryDispose(PickupObject pickup)
    {
        if (pickup == null)
            return;

        WasteItem waste = pickup.GetComponent<WasteItem>();

        if (waste == null)
        {
            Debug.LogWarning("This object has no WasteItem component.");
            return;
        }

        if (waste.category == acceptedCategory)
        {
            Debug.Log($"Correct bin! +{correctPoints} points");

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddPoints(correctPoints);
            }

            pickup.Dispose();
        }
        else
        {
            Debug.Log($"Wrong bin! {wrongPoints} points");

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddPoints(wrongPoints);
            }

            // Keep holding the object.
        }
    }
}