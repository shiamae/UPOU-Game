using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("Hint Popup")]
    public GameObject hintPanel;
    public Image hintImage;

    [Header("Result Popup")]
    public GameObject resultPanel;
    public Image resultImage;
    public Sprite correctResultImage;
    public Sprite wrongResultImage;
    public float popupDuration = 2f;

    [Header("Education Popup")]
    public GameObject educationPanel;
    public Image educationImage;

    [Header("Crosshair")]
    public GameObject crosshair;

    [Header("Feedback Effects")]
    public float correctFlashDuration = 0.2f;
    public float wrongFlashDuration = 0.22f;
    public float wrongShakeDuration = 0.18f;
    public float wrongShakeMagnitude = 0.12f;
    public Color correctFlashColor = new Color(1f, 1f, 1f, 0.25f);
    public Color wrongFlashColor = new Color(1f, 0.2f, 0.2f, 0.2f);

    private Coroutine popupRoutine;
    private Coroutine feedbackRoutine;
    private Image feedbackOverlay;

    // Waste waiting to be disposed
    private PickupObject pendingPickup;

    // Reference to the player pickup script
    private PlayerPickup playerPickup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        playerPickup = FindAnyObjectByType<PlayerPickup>();

        if (hintPanel != null)
            hintPanel.SetActive(false);

        if (resultPanel != null)
            resultPanel.SetActive(false);

        if (educationPanel != null)
            educationPanel.SetActive(false);

        if (feedbackOverlay == null)
        {
            GameObject overlayObject = new GameObject("FeedbackOverlay");
            overlayObject.transform.SetParent(transform, false);

            Canvas overlayCanvas = overlayObject.AddComponent<Canvas>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.sortingOrder = 1000;

            overlayObject.AddComponent<CanvasScaler>();
            overlayObject.AddComponent<GraphicRaycaster>();

            feedbackOverlay = overlayObject.AddComponent<Image>();
            feedbackOverlay.color = Color.clear;
            feedbackOverlay.rectTransform.anchorMin = Vector2.zero;
            feedbackOverlay.rectTransform.anchorMax = Vector2.one;
            feedbackOverlay.rectTransform.offsetMin = Vector2.zero;
            feedbackOverlay.rectTransform.offsetMax = Vector2.zero;
        }

        if (feedbackOverlay != null)
            feedbackOverlay.gameObject.SetActive(false);
    }

    //==================================================
    // HINT
    //==================================================

    public void ShowHint(WasteItem waste)
    {
        if (waste == null || waste.hintImage == null)
            return;

        hintImage.sprite = waste.hintImage;
        hintPanel.SetActive(true);
    }

    public void HideHint()
    {
        if (hintPanel != null)
            hintPanel.SetActive(false);
    }

    public void PlayPositiveFeedback()
    {
        if (feedbackOverlay == null)
            return;

        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        feedbackRoutine = StartCoroutine(PlayPositiveFeedbackRoutine());
    }

    public void PlayNegativeFeedback()
    {
        if (feedbackOverlay == null)
            return;

        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        feedbackRoutine = StartCoroutine(PlayNegativeFeedbackRoutine());
    }

    private IEnumerator PlayPositiveFeedbackRoutine()
    {
        feedbackOverlay.gameObject.SetActive(true);
        feedbackOverlay.color = correctFlashColor;

        float elapsed = 0f;
        while (elapsed < correctFlashDuration)
        {
            float alpha = Mathf.Lerp(correctFlashColor.a, 0f, elapsed / correctFlashDuration);
            feedbackOverlay.color = new Color(correctFlashColor.r, correctFlashColor.g, correctFlashColor.b, alpha);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        feedbackOverlay.color = Color.clear;
        feedbackOverlay.gameObject.SetActive(false);
    }

    private IEnumerator PlayNegativeFeedbackRoutine()
    {
        feedbackOverlay.gameObject.SetActive(true);
        feedbackOverlay.color = wrongFlashColor;

        Camera mainCamera = Camera.main;
        Vector3 originalPosition = mainCamera != null ? mainCamera.transform.localPosition : Vector3.zero;

        Handheld.Vibrate();

        float elapsed = 0f;
        while (elapsed < wrongShakeDuration)
        {
            float alpha = Mathf.Lerp(wrongFlashColor.a, 0f, elapsed / wrongShakeDuration);
            feedbackOverlay.color = new Color(wrongFlashColor.r, wrongFlashColor.g, wrongFlashColor.b, alpha);

            if (mainCamera != null)
            {
                Vector3 offset = Random.insideUnitSphere * wrongShakeMagnitude;
                offset.z = 0f;
                mainCamera.transform.localPosition = originalPosition + offset;
            }

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (mainCamera != null)
            mainCamera.transform.localPosition = originalPosition;

        feedbackOverlay.color = Color.clear;
        feedbackOverlay.gameObject.SetActive(false);
    }

    //==================================================
    // RESULT
    //==================================================

    public void ShowResult(bool correct, WasteItem waste = null, PickupObject pickup = null)
    {
        if (popupRoutine != null)
            StopCoroutine(popupRoutine);

        pendingPickup = pickup;

        resultPanel.SetActive(true);
        resultImage.sprite = correct ? correctResultImage : wrongResultImage;

        popupRoutine = StartCoroutine(ResultPopupRoutine(correct, waste));
    }

    private IEnumerator ResultPopupRoutine(bool correct, WasteItem waste)
    {
        // Uses realtime so it still works even if the game pauses later.
        yield return new WaitForSecondsRealtime(popupDuration);

        resultPanel.SetActive(false);

        // Wrong disposal ends here.
        if (!correct)
        {
            pendingPickup = null;
            yield break;
        }

        // Correct disposal -> show education popup.
        if (waste != null)
        {
            ShowEducation(waste);
        }
    }

    //==================================================
    // EDUCATION
    //==================================================

    public void ShowEducation(WasteItem waste)
    {
        if (waste == null || waste.educationImage == null)
            return;

        educationImage.sprite = waste.educationImage;
        educationPanel.SetActive(true);

        // Hide crosshair
        if (crosshair != null)
            crosshair.SetActive(false);

        // Pause the game
        Time.timeScale = 0f;

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseEducation()
    {
        educationPanel.SetActive(false);

        // Resume game
        Time.timeScale = 1f;

        // Show crosshair again
        if (crosshair != null)
            crosshair.SetActive(true);

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Destroy the waste now
        if (pendingPickup != null)
        {
            pendingPickup.Dispose();
            pendingPickup = null;
        }

        // Tell the player they are no longer holding anything
        if (playerPickup != null)
        {
            playerPickup.ClearHeldObject();
        }

        // Resume game
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Destroy the waste
        if (pendingPickup != null)
        {
            pendingPickup.Dispose();
            pendingPickup = null;
        }

        if (playerPickup != null)
        {
            playerPickup.ClearHeldObject();
        }

        // NOW show the badge
        if (BadgeManager.Instance != null)
        {
            BadgeManager.Instance.ShowPendingBadge();
        }
    }
}