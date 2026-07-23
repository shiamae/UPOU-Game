using UnityEngine;

public class TrashCan : MonoBehaviour
{
    [Header("Accepted Waste")]
    public WasteCategory acceptedCategory;

    [Header("Points")]
    public int correctPoints = 10;
    public int wrongPoints = -5;

    private void OnMouseEnter()
    {
        GameUIManager.Instance?.NotifyBinHoverEnter(this);
    }

    private void OnMouseExit()
    {
        GameUIManager.Instance?.NotifyBinHoverExit(this);
    }

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

        //==================================================
        // CORRECT BIN
        //==================================================
        if (waste.category == acceptedCategory)
        {
            Debug.Log($"Correct bin! +{correctPoints} points");

            // Add points
            ScoreManager.Instance?.AddPoints(correctPoints);

            // Update quest progress
            QuestManager.Instance?.RegisterRecycle(waste);

            // Play correct disposal sound
            AudioManager.Instance?.PlayCorrectDispose();

            // Hide hint panels
            GameUIManager.Instance?.HideHint();
            GameUIManager.Instance?.HideMiniInfo();

            // Positive feedback
            GameUIManager.Instance?.PlayPositiveFeedback();

            // Hide the held object immediately
            pickup.Hide();

            // ALWAYS let GameUIManager handle
            // whether this is the first time or not.
            GameUIManager.Instance?.ShowResult(true, waste, pickup);
        }

        //==================================================
        // WRONG BIN
        //==================================================
        else
        {
            Debug.Log($"Wrong bin! {wrongPoints} points");

            // Subtract points
            ScoreManager.Instance?.AddPoints(wrongPoints);

            // Play wrong disposal sound
            AudioManager.Instance?.PlayWrongDispose();

            GameUIManager.Instance?.PlayNegativeFeedback();
            GameUIManager.Instance?.ShowResult(false, null, null);

            // Keep holding the object so the player
            // can try another bin.
        }
    }
}