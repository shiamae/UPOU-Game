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

        //==================================
        // Correct bin
        //==================================
        if (waste.category == acceptedCategory)
        {
            Debug.Log($"Correct bin! +{correctPoints} points");

            // Add points
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddPoints(correctPoints);
            }

            // Play correct disposal sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCorrectDispose();
            }

            // Hide the hint popup
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.HideHint();
                GameUIManager.Instance.PlayPositiveFeedback();

                // Hide the object immediately
                pickup.Hide();

                // Show result popup
                // Education popup and final disposal
                // are handled by the GameUIManager.
                GameUIManager.Instance.ShowResult(true, waste, pickup);
            }
        }

        //==================================
        // Wrong bin
        //==================================
        else
        {
            Debug.Log($"Wrong bin! {wrongPoints} points");

            // Subtract points
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddPoints(wrongPoints);
            }

            // Play wrong disposal sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayWrongDispose();
            }

            // Show wrong result popup
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.PlayNegativeFeedback();
                GameUIManager.Instance.ShowResult(false, null, null);
            }

            // Keep holding the object so the player
            // can try another trash can.
        }
    }
}