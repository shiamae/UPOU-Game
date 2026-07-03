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

        playerPickup = FindFirstObjectByType<PlayerPickup>();

        hintPanel.SetActive(false);
        resultPanel.SetActive(false);
        educationPanel.SetActive(false);
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
        hintPanel.SetActive(false);
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
        // Show result popup
        yield return new WaitForSecondsRealtime(popupDuration);

        resultPanel.SetActive(false);

        // Wrong disposal ends here
        if (!correct)
        {
            pendingPickup = null;
            yield break;
        }

        // Show education popup
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
    }
}