using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("Hint Popup")]
    public GameObject hintPanel;
    public Image hintImage;
    public GameObject hintInstructionImage;

    [Header("Bin Hint Popup")]
    public GameObject binHintPanel;
    public Image binHintImage;
    public Sprite residualBinHintImage;
    public Sprite recyclableBinHintImage;
    public Sprite biodegradableBinHintImage;
    public Sprite hazardousBinHintImage;
    public Sprite infectiousBinHintImage;

    [Header("Result Popup")]
    public GameObject resultPanel;
    public Image resultImage;
    public Sprite correctResultImage;
    public Sprite wrongResultImage;
    public float popupDuration = 2f;

    [Header("Education Popup")]
    public GameObject educationPanel;
    public Image educationImage;

    [Header("Mini Info Panel")]
    public GameObject miniInfoPanel;
    public Image miniInfoImage;
    public GameObject miniInfoInstructionImage;

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

    private TrashCan hoveredBin;

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
        
        if (hintInstructionImage != null)
            hintInstructionImage.SetActive(false);

        if (binHintPanel != null)
            binHintPanel.SetActive(false);

        if (resultPanel != null)
            resultPanel.SetActive(false);

        if (educationPanel != null)
            educationPanel.SetActive(false);
        
        if (miniInfoPanel != null)
            miniInfoPanel.SetActive(false);
        
        if (miniInfoInstructionImage != null)
            miniInfoInstructionImage.SetActive(false);

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

    private void Update()
    {
        HandleCursorToggle();
        if (IsBinHintBlocked())
        {
            HideBinHint();
            return;
        }

        if (hoveredBin != null)
            ShowBinHint(hoveredBin.acceptedCategory);

        HandleBinHintInput();

        if (Keyboard.current != null &&
            Keyboard.current.fKey.wasPressedThisFrame)
        {
            ToggleWasteInfo();
        }
    }

    private void HandleBinHintInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame)
        {
            ShowBinHint(WasteCategory.Residual);
        }
        else if (keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame)
        {
            ShowBinHint(WasteCategory.Recyclable);
        }
        else if (keyboard.digit3Key.wasPressedThisFrame || keyboard.numpad3Key.wasPressedThisFrame)
        {
            ShowBinHint(WasteCategory.Biodegradable);
        }
        else if (keyboard.digit4Key.wasPressedThisFrame || keyboard.numpad4Key.wasPressedThisFrame)
        {
            ShowBinHint(WasteCategory.Hazardous);
        }
        else if (keyboard.digit5Key.wasPressedThisFrame || keyboard.numpad5Key.wasPressedThisFrame)
        {
            ShowBinHint(WasteCategory.Infectious);
        }
        else if (keyboard.escapeKey.wasPressedThisFrame)
        {
            HideBinHint();
        }
    }

    public void NotifyBinHoverEnter(TrashCan bin)
    {
        if (bin == null)
            return;

        hoveredBin = bin;

        if (!IsBinHintBlocked())
            ShowBinHint(bin.acceptedCategory);
    }

    public void NotifyBinHoverExit(TrashCan bin)
    {
        if (bin == null || hoveredBin != bin)
            return;

        hoveredBin = null;
        HideBinHint();
    }

    public void ShowBinHint(WasteCategory category)
    {
        if (IsBinHintBlocked())
        {
            HideBinHint();
            return;
        }

        ShowBinHint(GetBinHintSprite(category));
    }

    private Sprite GetBinHintSprite(WasteCategory category)
    {
        switch (category)
        {
            case WasteCategory.Recyclable:
                return recyclableBinHintImage;
            case WasteCategory.Biodegradable:
                return biodegradableBinHintImage;
            case WasteCategory.Residual:
                return residualBinHintImage;
            case WasteCategory.Hazardous:
                return hazardousBinHintImage;
            case WasteCategory.Infectious:
                return infectiousBinHintImage;
            default:
                return null;
        }
    }

    private void ShowBinHint(Sprite sprite)
    {
        if (sprite == null)
        {
            HideBinHint();
            return;
        }

        if (binHintPanel == null || binHintImage == null)
        {
            Debug.LogWarning("GameUIManager: Bin hint panel or image is not assigned.");
            return;
        }

        binHintImage.sprite = sprite;
        binHintPanel.SetActive(true);
    }

    public void HideBinHint()
    {
        if (binHintPanel != null)
            binHintPanel.SetActive(false);
    }

    private bool IsLandingPageActive()
    {
        return MiniGameDomain.IsAnyLandingPageShowing;
    }

    private bool IsBadgePopupActive()
    {
        return BadgeManager.Instance != null && BadgeManager.Instance.IsBadgePopupShowing;
    }

    private bool IsGamePopupActive()
    {
        return (resultPanel != null && resultPanel.activeInHierarchy) ||
               (educationPanel != null && educationPanel.activeInHierarchy);
    }

    private bool IsBinHintBlocked()
    {
        return IsLandingPageActive() || IsBadgePopupActive() || IsGamePopupActive();
    }

    //==================================================
    // HINT
    //==================================================

    public void ShowHint(WasteItem waste)
    {
        if (waste == null || waste.hintImage == null)
            return;

        HideMiniInfo();

        hintImage.sprite = waste.hintImage;
        hintPanel.SetActive(true);

        if (hintInstructionImage != null)
            hintInstructionImage.SetActive(true);
    }

    public void HideHint()
    {
        if (hintPanel != null)
            hintPanel.SetActive(false);

        if (hintInstructionImage != null)
            hintInstructionImage.SetActive(false);
    }

    public void ShowMiniInfo(WasteItem waste)
    {
        if (waste == null || waste.educationImage == null)
            return;

        HideHint();

        miniInfoImage.sprite = waste.educationImage;
        miniInfoPanel.SetActive(true);

        if (miniInfoInstructionImage != null)
            miniInfoInstructionImage.SetActive(true);
    }

    public void HideMiniInfo()
    {
        if (miniInfoPanel != null)
            miniInfoPanel.SetActive(false);

        if (miniInfoInstructionImage != null)
            miniInfoInstructionImage.SetActive(false);
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

        #if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
        #endif

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

        if (waste == null)
            yield break;

        bool firstTime =
            WasteLearningManager.Instance != null &&
            !WasteLearningManager.Instance.HasLearned(waste.wasteType);

        if (firstTime)
        {
            // First successful recycle of this waste.
            WasteLearningManager.Instance.Learn(waste.wasteType);

            ShowEducation(waste);
        }
        else
        {
            // Already learned.
            // Skip the education popup.
            HideMiniInfo();

            if (pendingPickup != null)
            {
                pendingPickup.Dispose();
                pendingPickup = null;
            }

            if (playerPickup != null)
            {
                playerPickup.ClearHeldObject();
            }

            // Badge system should still run.
            BadgeManager.Instance?.ShowPendingBadge();
        }
    }

    //==================================================
    // EDUCATION
    //==================================================

    public void ShowEducation(WasteItem waste)
    {
        HideMiniInfo();
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

    public void CloseMiniInfo()
    {
        HideMiniInfo();
    }

    public void ToggleWasteInfo()
    {
        WasteItem waste = PlayerPickup.Instance?.HeldWasteItem;

        if (waste == null)
            return;

        // Play button click every time F is pressed
        AudioManager.Instance?.PlayButtonClick();

        bool learned =
            WasteLearningManager.Instance != null &&
            WasteLearningManager.Instance.HasLearned(waste.wasteType);

        if (learned)
        {
            // Toggle Mini Info
            if (miniInfoPanel.activeSelf)
            {
                HideMiniInfo();
            }
            else
            {
                ShowMiniInfo(waste);
            }
        }
        else
        {
            // Toggle Hint
            if (hintPanel.activeSelf)
            {
                HideHint();
            }
            else
            {
                ShowHint(waste);
            }
        }
    }

    private void HandleCursorToggle()
    {
        if (Keyboard.current == null)
            return;

        if (!Keyboard.current.escapeKey.wasPressedThisFrame)
            return;

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
