using System.Collections;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    [Header("Accepted Waste")]
    public WasteCategory acceptedCategory;

    [Header("Points")]
    public int correctPoints = 10;
    public int wrongPoints = -5;

    [Header("Feedback")]
    public float feedbackDuration = 0.3f;
    public float correctScaleBoost = 0.25f;
    public float wrongScaleBoost = 0.16f;
    public float screenShakeIntensity = 0.08f;

    private Coroutine feedbackRoutine;

    private IEnumerator FeedbackRoutine(bool correct)
    {
        Vector3 originalScale = transform.localScale;
        Quaternion originalRotation = transform.localRotation;
        Camera mainCamera = Camera.main;
        Vector3 originalCameraPosition = mainCamera != null ? mainCamera.transform.localPosition : Vector3.zero;

        float elapsed = 0f;
        float targetBoost = correct ? correctScaleBoost : wrongScaleBoost;

        while (elapsed < feedbackDuration)
        {
            float t = elapsed / feedbackDuration;
            float pulse = Mathf.Sin(t * Mathf.PI * 3f);
            float bounceAmount = Mathf.Abs(pulse) * targetBoost;
            float sparkleScale = 1f + (correct ? bounceAmount : bounceAmount * 0.7f);
            transform.localScale = originalScale * sparkleScale;

            if (correct)
            {
                float sparkleRotation = Mathf.Sin(t * 30f) * 10f;
                transform.localRotation = originalRotation * Quaternion.Euler(0f, 0f, sparkleRotation);
            }
            else
            {
                transform.localRotation = originalRotation * Quaternion.Euler(0f, 0f, Mathf.Sin(t * 18f) * -10f);
            }

            if (mainCamera != null)
            {
                Vector3 shakeOffset = Random.insideUnitSphere * (correct ? screenShakeIntensity * 0.45f : screenShakeIntensity);
                mainCamera.transform.localPosition = originalCameraPosition + shakeOffset;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        transform.localRotation = originalRotation;

        if (mainCamera != null)
        {
            mainCamera.transform.localPosition = originalCameraPosition;
        }
    }

    private void TriggerFeedback(bool correct)
    {
        if (feedbackRoutine != null)
        {
            StopCoroutine(feedbackRoutine);
        }

        feedbackRoutine = StartCoroutine(FeedbackRoutine(correct));
    }

    public string GetDisplayName()
    {
        switch (acceptedCategory)
        {
            case WasteCategory.Recyclable:
                return "Recyclable Bin";
            case WasteCategory.Biodegradable:
                return "Biodegradable Bin";
            case WasteCategory.Residual:
                return "Residual Bin";
            case WasteCategory.Hazardous:
                return "Hazardous Bin";
            case WasteCategory.Infectious:
                return "Infectious Bin";
            default:
                return "Trash Bin";
        }
    }

    public string GetDescription()
    {
        switch (acceptedCategory)
        {
            case WasteCategory.Recyclable:
                return "For materials that belong in the recycling stream.";
            case WasteCategory.Biodegradable:
                return "For waste that can naturally break down.";
            case WasteCategory.Residual:
                return "For general non-recyclable waste.";
            case WasteCategory.Hazardous:
                return "For waste that needs special handling.";
            case WasteCategory.Infectious:
                return "For biohazard waste that requires careful disposal.";
            default:
                return "Use this bin for the matching waste type.";
        }
    }

    /// <summary>
    /// Returns a hint about what this trash can accepts, without revealing specific items.
    /// </summary>
    public string GetHint()
    {
        switch (acceptedCategory)
        {
            case WasteCategory.Recyclable:
                return "💡 HINT: This bin accepts materials that can be processed and reused. Look for items made of paper, plastic, or metal.";
            case WasteCategory.Biodegradable:
                return "💡 HINT: This bin is for organic waste. Think about materials from nature that decompose naturally.";
            case WasteCategory.Residual:
                return "💡 HINT: This bin accepts general waste that doesn't fit other categories. Items that are difficult to recycle go here.";
            case WasteCategory.Hazardous:
                return "💡 HINT: This bin is for dangerous materials. Look for items that require special handling and safety precautions.";
            case WasteCategory.Infectious:
                return "💡 HINT: This bin is for biohazard waste. Only certain medical or biological materials belong here.";
            default:
                return "💡 HINT: Choose wisely based on what this bin accepts.";
        }
    }

    /// <summary>
    /// Returns examples of what this trash can accepts (without naming the current waste item).
    /// </summary>
    public string GetCategoryExamples()
    {
        switch (acceptedCategory)
        {
            case WasteCategory.Recyclable:
                return "Examples: Cardboard, glass bottles, aluminum cans, plastic bottles";
            case WasteCategory.Biodegradable:
                return "Examples: Food scraps, leaves, grass, food waste";
            case WasteCategory.Residual:
                return "Examples: Non-recyclable packaging, ceramics, broken items";
            case WasteCategory.Hazardous:
                return "Examples: Batteries, electronics, chemicals, paint";
            case WasteCategory.Infectious:
                return "Examples: Medical waste, contaminated materials, sharps";
            default:
                return "Check the bin label for accepted items.";
        }
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

            TriggerFeedback(true);

            // Hide the hint popup
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.HideHint();

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

            TriggerFeedback(false);

            // Show wrong result popup
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.ShowResult(false, null, null);
            }

            // Keep holding the object so the player
            // can try another trash can.
        }
    }
}