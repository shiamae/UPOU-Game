using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Waste/Recyclable Panel (bottom)")]
    [Tooltip("Assign each slot's child 'Icon' Image, in order. Slot 0 = currently held waste item. Remaining slots are reserved for the future recycling feature.")]
    public Image[] wasteSlots;

    [Header("Badge Panel (left side)")]
    [Tooltip("Assign each slot's child 'Icon' Image, in order. Fills in with unlocked badges as they're earned.")]
    public Image[] badgeSlots;

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
    }

    private void Start()
    {
        RefreshHeldItemSlot();
        RefreshBadgeSlots();
    }

    private void Update()
    {
        // Held item changes constantly (pickup / drop / dispose),
        // so keep it live every frame.
        RefreshHeldItemSlot();
    }

    //==================================================
    // Waste Panel - Slot 0 = Held Item
    //==================================================

    public void RefreshHeldItemSlot()
    {
        if (wasteSlots == null || wasteSlots.Length == 0 || wasteSlots[0] == null)
            return;

        WasteItem waste = PlayerPickup.Instance != null ? PlayerPickup.Instance.HeldWasteItem : null;
        SetSlotIcon(wasteSlots[0], waste != null ? waste.inventoryIcon : null);
    }

    //==================================================
    // Badge Panel - Called by BadgeManager on unlock
    //==================================================

    public void RefreshBadgeSlots()
    {
        if (badgeSlots == null || BadgeManager.Instance == null)
            return;

        int slotIndex = 0;

        foreach (BadgeManager.Badge badge in BadgeManager.Instance.badges)
        {
            if (!badge.unlocked)
                continue;

            if (slotIndex >= badgeSlots.Length)
                break;

            SetSlotIcon(badgeSlots[slotIndex], badge.badgeIcon);
            slotIndex++;
        }
    }

    //==================================================
    // Helper - shows/hides an icon without touching the slot frame
    //==================================================

    private void SetSlotIcon(Image iconImage, Sprite sprite)
    {
        if (iconImage == null)
            return;

        iconImage.sprite = sprite;

        // Fade alpha instead of disabling, so the slot frame
        // stays visible and layout doesn't shift.
        Color c = iconImage.color;
        c.a = sprite != null ? 1f : 0f;
        iconImage.color = c;
    }
}