using UnityEngine;
using UnityEngine.InputSystem;

public class HeldCraftItemUI : MonoBehaviour
{
    public static HeldCraftItemUI Instance;

    [Header("Held Item Images")]
    public GameObject notebookImage;
    public GameObject pencilHolderImage;
    public GameObject deskOrganizerImage;

    // 1 = Waste
    // 2 = Notebook
    // 3 = Pencil Holder
    // 4 = Desk Organizer
    private int currentSlot = 1;

    public int CurrentSlot => currentSlot;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        HideAll();
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Time.timeScale == 0f)
            return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll > 0f)
            PreviousSlot();
        else if (scroll < 0f)
            NextSlot();
    }

    //==================================================
    // Slot Switching
    //==================================================

    private void NextSlot()
    {
        int startSlot = currentSlot;

        do
        {
            currentSlot++;

            if (currentSlot > 4)
                currentSlot = 1;

        } while (!IsSlotAvailable(currentSlot) && currentSlot != startSlot);

        AudioManager.Instance?.PlayInventoryScroll();
        EquipCurrentSlot();
    }

    private void PreviousSlot()
    {
        int startSlot = currentSlot;

        do
        {
            currentSlot--;

            if (currentSlot < 1)
                currentSlot = 4;

        } while (!IsSlotAvailable(currentSlot) && currentSlot != startSlot);

        AudioManager.Instance?.PlayInventoryScroll();
        EquipCurrentSlot();
    }

    //==================================================
    // Equip
    //==================================================

    private void EquipCurrentSlot()
    {
        HideAll();

        PickupObject held = PlayerPickup.Instance?.HeldObject;

        if (held != null)
        {
            bool usingWasteSlot = currentSlot == 1;

            held.SetHeldVisible(usingWasteSlot);
            held.SetCanThrow(usingWasteSlot);

            WasteItem waste = held.GetComponent<WasteItem>();

            if (GameUIManager.Instance != null)
            {
                if (usingWasteSlot)
                {
                    if (waste != null)
                    {
                        if (WasteLearningManager.Instance != null &&
                            WasteLearningManager.Instance.HasLearned(waste.wasteType))
                        {
                            // Already learned
                            GameUIManager.Instance.HideHint();
                            GameUIManager.Instance.ShowMiniInfo(waste);
                        }
                        else
                        {
                            // First time
                            GameUIManager.Instance.HideMiniInfo();
                            GameUIManager.Instance.ShowHint(waste);
                        }
                    }
                }
                else
                {
                    // Leaving the waste slot
                    GameUIManager.Instance.HideHint();
                    GameUIManager.Instance.HideMiniInfo();
                }
            }
        }
        else
        {
            // No held waste
            GameUIManager.Instance?.HideHint();
            GameUIManager.Instance?.HideMiniInfo();
        }

        switch (currentSlot)
        {
            case 1:
                // Waste handled above
                break;

            case 2:
                if (notebookImage != null)
                    notebookImage.SetActive(true);
                break;

            case 3:
                if (pencilHolderImage != null)
                    pencilHolderImage.SetActive(true);
                break;

            case 4:
                if (deskOrganizerImage != null)
                    deskOrganizerImage.SetActive(true);
                break;
        }

        Debug.Log("Current Slot: " + currentSlot);
    }

    public void SwitchToWasteSlot()
    {
        currentSlot = 1;
        EquipCurrentSlot();
    }

    //==================================================
    // Availability
    //==================================================

    private bool IsSlotAvailable(int slot)
    {
        switch (slot)
        {
            case 1:
                return true;

            case 2:
                return CraftedInventoryManager.Instance != null &&
                       CraftedInventoryManager.Instance.NotebookUnlocked;

            case 3:
                return CraftedInventoryManager.Instance != null &&
                       CraftedInventoryManager.Instance.PencilHolderUnlocked;

            case 4:
                return CraftedInventoryManager.Instance != null &&
                       CraftedInventoryManager.Instance.DeskOrganizerUnlocked;

            default:
                return false;
        }
    }

    //==================================================
    // Helpers
    //==================================================

    public void HideAll()
    {
        if (notebookImage != null)
            notebookImage.SetActive(false);

        if (pencilHolderImage != null)
            pencilHolderImage.SetActive(false);

        if (deskOrganizerImage != null)
            deskOrganizerImage.SetActive(false);
    }
}