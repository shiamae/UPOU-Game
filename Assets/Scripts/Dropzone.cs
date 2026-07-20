using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    [Header("Accepted Item")]
    public string acceptedItemID;

    [Header("Craft Step")]
    public CraftStep craftStep;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        DraggableItem draggable = eventData.pointerDrag.GetComponent<DraggableItem>();

        if (draggable == null)
            return;

        // Wrong item
        if (draggable.itemID != acceptedItemID)
        {
            Debug.Log("Wrong item dropped.");
            return;
        }

        Debug.Log($"Correct item dropped! ({draggable.itemID})");

        // Snap the dragged item into this drop zone
        draggable.SnapTo(transform);

        // Advance the crafting process
        switch (craftStep)
        {
            //==================================
            // Notebook
            //==================================

            case CraftStep.PunchPaper:
                CraftingManager.Instance?.PunchPaper();
                break;

            case CraftStep.AssembleNotebook:
                CraftingManager.Instance?.AssembleNotebook();
                break;

            //==================================
            // Pencil Holder
            //==================================

            case CraftStep.CutBottle:
                CraftingManager.Instance?.CutBottle();
                break;

            case CraftStep.CleanBottle:
                CraftingManager.Instance?.CleanBottle();
                break;

            case CraftStep.DecorateBottle:
                CraftingManager.Instance?.DecorateBottle();
                break;

            default:
                Debug.LogWarning("Unhandled CraftStep: " + craftStep);
                break;

            //==================================
            // Desk Organizer
            //==================================

            case CraftStep.CutMilkCarton1:
                CraftingManager.Instance?.CutFirstMilkCarton();
                break;

            case CraftStep.CutMilkCarton2:
                CraftingManager.Instance?.CutSecondMilkCarton();
                break;

            case CraftStep.GlueDeskOrganizer:
                CraftingManager.Instance?.GlueDeskOrganizer();
                break;
        }
    }
}