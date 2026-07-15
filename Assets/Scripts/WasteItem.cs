using UnityEngine;

public class WasteItem : MonoBehaviour
{
    public string itemName;

    public WasteCategory category;

    [Header("UI Images")]
    public Sprite hintImage;
    public Sprite educationImage;

    [Header("Inventory")]
    [Tooltip("A clean 2D icon representing this item, shown in the inventory slot.")]
    public Sprite inventoryIcon;
}