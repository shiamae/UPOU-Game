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

        // Correct bin
        if (waste.category == acceptedCategory)
        {
            Debug.Log($"Correct bin! +{correctPoints} points");

            // Add score
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddPoints(correctPoints);
            }

            // Show temporary result popup
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.ShowResult(true);

                // Hide the hint popup
                GameUIManager.Instance.HideHint();

                // Show the educational popup
                GameUIManager.Instance.ShowEducation(waste);
            }

            // Destroy the object
            pickup.Dispose();
        }
        // Wrong bin
        else
        {
            Debug.Log($"Wrong bin! {wrongPoints} points");

            // Subtract score
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddPoints(wrongPoints);
            }

            // Show temporary result popup
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.ShowResult(false);
            }

            // Do NOT destroy the object.
            // The player continues holding it and can try another bin.
        }
    }
}