using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RecyclableCollectionUI : MonoBehaviour
{
    public static RecyclableCollectionUI Instance;

    [Header("UI")]
    public GameObject collectionPanel;

    [Header("Quest Panel")]
    public QuestPanelUI questPanelUI;

    [Header("Crosshair")]
    public GameObject crosshair;

    //==================================================
    // Collection Buttons
    //==================================================

    [Header("Notebook")]
    public Button notebookButton;
    public Image notebookLockOverlay;

    [Header("Pencil Holder")]
    public Button pencilHolderButton;
    public Image pencilHolderLockOverlay;

    [Header("Desk Organizer")]
    public Button deskOrganizerButton;
    public Image deskOrganizerLockOverlay;

    private bool isOpen;

    public bool IsOpen => isOpen;

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

        if (collectionPanel != null)
            collectionPanel.SetActive(false);

        InitializeCollection();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        // Don't allow opening while another popup has paused the game
        if (Time.timeScale == 0f && !isOpen)
            return;

        if (!isOpen && Keyboard.current.rKey.wasPressedThisFrame)
        {
            OpenCollection();
        }
    }

    //==================================================
    // Open / Close Collection
    //==================================================

    public void OpenCollection()
    {
        if (collectionPanel == null)
            return;

        AudioManager.Instance?.PlayButtonClick();

        collectionPanel.SetActive(true);

        questPanelUI?.HidePanel();

        if (crosshair != null)
            crosshair.SetActive(false);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isOpen = true;
    }

    // Assign this to the X button
    public void CloseCollection()
    {
        if (collectionPanel == null)
            return;

        AudioManager.Instance?.PlayButtonClick();

        collectionPanel.SetActive(false);

        questPanelUI?.ShowPanel();

        if (crosshair != null)
            crosshair.SetActive(true);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isOpen = false;
    }

    //==================================================
    // Initialization
    //==================================================

    private void InitializeCollection()
    {
        LockNotebook();
        LockPencilHolder();
        LockDeskOrganizer();
    }

    //==================================================
    // Unlocks
    //==================================================

    public void UnlockNotebook()
    {
        SetButtonState(notebookButton, notebookLockOverlay, true);
    }

    public void UnlockPencilHolder()
    {
        SetButtonState(pencilHolderButton, pencilHolderLockOverlay, true);
    }

    public void UnlockDeskOrganizer()
    {
        SetButtonState(deskOrganizerButton, deskOrganizerLockOverlay, true);
    }

    //==================================================
    // Locks
    //==================================================

    public void LockNotebook()
    {
        SetButtonState(notebookButton, notebookLockOverlay, false);
    }

    public void LockPencilHolder()
    {
        SetButtonState(pencilHolderButton, pencilHolderLockOverlay, false);
    }

    public void LockDeskOrganizer()
    {
        SetButtonState(deskOrganizerButton, deskOrganizerLockOverlay, false);
    }

    //==================================================
    // Helper
    //==================================================

    private void SetButtonState(Button button, Image overlay, bool unlocked)
    {
        if (button != null)
            button.interactable = unlocked;

        if (overlay != null)
            overlay.gameObject.SetActive(!unlocked);
    }
}