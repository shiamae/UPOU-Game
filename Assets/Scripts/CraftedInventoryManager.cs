using UnityEngine;
using UnityEngine.UI;

public class CraftedInventoryManager : MonoBehaviour
{
    public static CraftedInventoryManager Instance;

    [Header("Inventory Slots")]
    public Image slot2Image;
    public Image slot3Image;
    public Image slot4Image;

    [Header("Locked Icons")]
    public Sprite lockedSprite;

    [Header("Crafted Item Icons")]
    public Sprite notebookSprite;
    public Sprite pencilHolderSprite;
    public Sprite deskOrganizerSprite;

    private bool notebookUnlocked;
    private bool pencilHolderUnlocked;
    private bool deskOrganizerUnlocked;

    public bool NotebookUnlocked => notebookUnlocked;
    public bool PencilHolderUnlocked => pencilHolderUnlocked;
    public bool DeskOrganizerUnlocked => deskOrganizerUnlocked;

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

        InitializeInventory();
    }

    //==================================================
    // Initialization
    //==================================================

    private void InitializeInventory()
    {
        notebookUnlocked = false;
        pencilHolderUnlocked = false;
        deskOrganizerUnlocked = false;

        if (slot2Image != null)
            slot2Image.sprite = lockedSprite;

        if (slot3Image != null)
            slot3Image.sprite = lockedSprite;

        if (slot4Image != null)
            slot4Image.sprite = lockedSprite;
    }

    //==================================================
    // Notebook
    //==================================================

    public void UnlockNotebook()
    {
        if (notebookUnlocked)
            return;

        notebookUnlocked = true;

        if (slot2Image != null)
            slot2Image.sprite = notebookSprite;

        Debug.Log("Notebook unlocked!");
    }

    //==================================================
    // Pencil Holder
    //==================================================

    public void UnlockPencilHolder()
    {
        if (pencilHolderUnlocked)
            return;

        pencilHolderUnlocked = true;

        if (slot3Image != null)
            slot3Image.sprite = pencilHolderSprite;

        Debug.Log("Pencil Holder unlocked!");
    }

    //==================================================
    // Desk Organizer
    //==================================================

    public void UnlockDeskOrganizer()
    {
        if (deskOrganizerUnlocked)
            return;

        deskOrganizerUnlocked = true;

        if (slot4Image != null)
            slot4Image.sprite = deskOrganizerSprite;

        Debug.Log("Desk Organizer unlocked!");
    }

    //==================================================
    // Helpers
    //==================================================

    public bool IsUnlocked(int slot)
    {
        switch (slot)
        {
            case 2:
                return notebookUnlocked;

            case 3:
                return pencilHolderUnlocked;

            case 4:
                return deskOrganizerUnlocked;

            default:
                return false;
        }
    }
}