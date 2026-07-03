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

        //=========================
        // Correct bin
        //=========================
        if (waste.category == acceptedCategory)
        {
            Debug.Log($"Correct bin! +{correctPoints} points");

            // Add points
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddPoints(correctPoints);
            }

            // Hide the hint popup
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.HideHint();

                // Show result popup.
                // GameUIManager will later show the education popup
                // and dispose of the object after it is closed.
                pickup.Hide();
                GameUIManager.Instance.ShowResult(true, waste, pickup);
            }

            // IMPORTANT:
            // Do NOT call pickup.Dispose() here.
        }

        //=========================
        // Wrong bin
        //=========================
        else
        {
            Debug.Log($"Wrong bin! {wrongPoints} points");

            // Subtract points
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddPoints(wrongPoints);
            }

            // Show wrong popup only.
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.ShowResult(false, null, null);
            }

            // Keep holding the object.
        }
    }
}