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

    [Header("Trash Bin Info")]
    public GameObject trashBinInfoPanel;
    public Text trashBinTitleText;
    public Text trashBinDescriptionText;
    public Text trashBinHintText;
    public Text trashBinExamplesText;

    private Coroutine popupRoutine;

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

        if (trashBinInfoPanel != null)
            trashBinInfoPanel.SetActive(false);
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

    //==================================================
    // TRASH BIN INFO
    //==================================================

    public void ShowTrashBinInfo(TrashCan trashCan)
    {
        if (trashCan == null)
        {
            HideTrashBinInfo();
            return;
        }

        if (trashBinInfoPanel != null)
            trashBinInfoPanel.SetActive(true);

        if (trashBinTitleText != null)
            trashBinTitleText.text = trashCan.GetDisplayName();

        if (trashBinDescriptionText != null)
            trashBinDescriptionText.text = trashCan.GetDescription();

        // Show hint to help player decide
        if (trashBinHintText != null)
            trashBinHintText.text = trashCan.GetHint();

        // Show examples of what belongs in this bin
        if (trashBinExamplesText != null)
            trashBinExamplesText.text = trashCan.GetCategoryExamples();
    }

    public void HideTrashBinInfo()
    {
        if (trashBinInfoPanel != null)
            trashBinInfoPanel.SetActive(false);
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