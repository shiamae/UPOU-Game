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

        // Don't allow opening while another popup paused the game
        if (Time.timeScale == 0f && !isOpen)
            return;

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            if (isOpen)
                CloseCollection();
            else
                OpenCollection();
        }
    }

    //==================================================
    // OPEN / CLOSE
    //==================================================

    public void OpenCollection()
    {
        if (collectionPanel == null)
            return;

        AudioManager.Instance?.PlayButtonClick();

        // Hide any waste UI while the collection is open
        GameUIManager.Instance?.HideHint();
        GameUIManager.Instance?.HideMiniInfo();

        collectionPanel.SetActive(true);

        questPanelUI?.HidePanel();

        if (crosshair != null)
            crosshair.SetActive(false);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isOpen = true;
    }

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

        // Restore the correct waste UI if the player is holding waste
        PickupObject held = PlayerPickup.Instance?.HeldObject;

        if (held != null && HeldCraftItemUI.Instance != null &&
            HeldCraftItemUI.Instance.CurrentSlot == 1)
        {
            WasteItem waste = held.GetComponent<WasteItem>();

            if (waste != null)
            {
                if (WasteLearningManager.Instance.HasLearned(waste.wasteType))
                    GameUIManager.Instance?.ShowMiniInfo(waste);
                else
                    GameUIManager.Instance?.ShowHint(waste);
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isOpen = false;
    }

    //==================================================
    // INITIALIZATION
    //==================================================

    private void InitializeCollection()
    {
        LockNotebook();
        LockPencilHolder();
        LockDeskOrganizer();
    }

    //==================================================
    // UNLOCKS
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
    // LOCKS
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
    // COLLECTION BUTTONS
    //==================================================

    public void OpenNotebook()
    {
        AudioManager.Instance?.PlayButtonClick();
        collectionPanel.SetActive(false);
        CraftingManager.Instance?.OpenNotebookCraft();
    }

    public void OpenPencilHolder()
    {
        Debug.Log("OpenPencilHolder() called");
        AudioManager.Instance?.PlayButtonClick();
        collectionPanel.SetActive(false);
        Debug.Log("CraftingManager = " + CraftingManager.Instance);
        CraftingManager.Instance?.OpenPencilHolderCraft();
    }

    public void OpenDeskOrganizer()
    {
        AudioManager.Instance?.PlayButtonClick();
        collectionPanel.SetActive(false);
        CraftingManager.Instance?.OpenDeskOrganizerCraft();
    }

    //==================================================
    // HELPER
    //==================================================

    private void SetButtonState(Button button, Image overlay, bool unlocked)
    {
        if (button != null)
            button.interactable = unlocked;

        if (overlay != null)
            overlay.gameObject.SetActive(!unlocked);
    }
}