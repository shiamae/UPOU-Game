using UnityEngine;
using UnityEngine.InputSystem;

public class QuestPanelUI : MonoBehaviour
{
    [Header("Panel")]
    public RectTransform questPanel;

    [Header("Positions")]
    public Vector2 openPosition;
    public Vector2 closedPosition;

    [Header("Animation")]
    public float slideSpeed = 10f;

    private bool isOpen = true;
    private bool isHidden = false;
    private Vector2 targetPosition;

    private void Start()
    {
        targetPosition = openPosition;

        if (questPanel != null)
            questPanel.anchoredPosition = openPosition;
    }

    private void Update()
    {
        // Don't allow toggling while another popup is open
        if (Time.timeScale == 0f)
            return;

        // Don't allow toggling while the panel is hidden
        if (isHidden)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.qKey.wasPressedThisFrame)
        {
            TogglePanel();
        }

        if (questPanel == null)
            return;

        questPanel.anchoredPosition = Vector2.Lerp(
            questPanel.anchoredPosition,
            targetPosition,
            slideSpeed * Time.deltaTime);
    }

    public void TogglePanel()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        isOpen = !isOpen;
        targetPosition = isOpen ? openPosition : closedPosition;
    }

    public void OpenPanel()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        isOpen = true;
        targetPosition = openPosition;
    }

    public void ClosePanel()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        isOpen = false;
        targetPosition = closedPosition;
    }

    /// <summary>
    /// Temporarily hides the quest panel.
    /// Used when another full-screen UI (like the recyclable collection) is open.
    /// </summary>
    public void HidePanel()
    {
        if (questPanel == null)
            return;

        isHidden = true;
        questPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows the quest panel again.
    /// Restores it to the same open/closed state it had previously.
    /// </summary>
    public void ShowPanel()
    {
        if (questPanel == null)
            return;

        isHidden = false;
        questPanel.gameObject.SetActive(true);

        targetPosition = isOpen ? openPosition : closedPosition;
        questPanel.anchoredPosition = targetPosition;
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}