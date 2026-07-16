using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;

    [Header("Popup")]
    public GameObject craftingPopup;

    [Header("Craft Panels")]
    public GameObject notebookPanel;
    public GameObject pencilHolderPanel;

    //==================================================
    // NOTEBOOK
    //==================================================

    [Header("Notebook")]
    public GameObject puncher;
    public GameObject paper;
    public GameObject paperWithHoles;
    public GameObject notebookCover;
    public GameObject finishedNotebook;

    //==================================================
    // PENCIL HOLDER
    //==================================================

    [Header("Pencil Holder")]
    public GameObject scissors;
    public GameObject dirtyBottle;
    public GameObject cutBottle;
    public GameObject sponge;
    public GameObject cleanBottle;
    public GameObject coloredPaper;
    public GameObject finishedPencilHolder;

    //==================================================
    // DESK ORGANIZER
    //==================================================

    [Header("Desk Organizer")]
    public GameObject deskOrganizerPanel;

    // Step 1
    public GameObject scissors1;
    public GameObject milkCarton1;

    // Step 2
    public GameObject scissors2;
    public GameObject milkCarton2;

    // Step 3
    public GameObject glue;
    public GameObject twoCutCartons;

    // Final
    public GameObject finishedDeskOrganizer;

    [Header("Completion")]
    public GameObject completionPanel;

    private int currentStep;

    private DraggableItem[] draggableItems;

    private class DraggableState
    {
        public DraggableItem item;
        public Transform parent;
        public int siblingIndex;
        public Vector2 anchoredPosition;
    }

    private readonly List<DraggableState> draggableStates = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (craftingPopup != null)
            craftingPopup.SetActive(false);
    }

    private void Start()
    {
        draggableItems = craftingPopup.GetComponentsInChildren<DraggableItem>(true);

        draggableStates.Clear();

        foreach (DraggableItem item in draggableItems)
        {
            RectTransform rt = item.GetComponent<RectTransform>();

            draggableStates.Add(new DraggableState
            {
                item = item,
                parent = item.transform.parent,
                siblingIndex = item.transform.GetSiblingIndex(),
                anchoredPosition = rt.anchoredPosition
            });
        }
    }

    //==================================================
    // OPEN NOTEBOOK
    //==================================================

    public void OpenNotebookCraft()
    {
        Debug.Log("Opening Notebook Craft");

        craftingPopup.SetActive(true);

        notebookPanel.SetActive(true);
        pencilHolderPanel.SetActive(false);
        deskOrganizerPanel.SetActive(false);

        ResetCraft();
    }

    //==================================================
    // OPEN PENCIL HOLDER
    //==================================================

    public void OpenPencilHolderCraft()
    {
        craftingPopup.SetActive(true);

        notebookPanel.SetActive(false);
        pencilHolderPanel.SetActive(true);
        deskOrganizerPanel.SetActive(false);

        ResetCraft();
    }

    public void OpenDeskOrganizerCraft()
    {
        craftingPopup.SetActive(true);

        notebookPanel.SetActive(false);
        pencilHolderPanel.SetActive(false);
        deskOrganizerPanel.SetActive(true);

        ResetCraft();
    }

    //==================================================
    // NOTEBOOK STEP 1
    //==================================================

    public void PunchPaper()
    {
        if (currentStep != 0)
            return;

        currentStep = 1;

        puncher.SetActive(false);
        paper.SetActive(false);

        paperWithHoles.SetActive(true);
        notebookCover.SetActive(true);
    }

    //==================================================
    // NOTEBOOK STEP 2
    //==================================================

    public void AssembleNotebook()
    {
        if (currentStep != 1)
            return;

        currentStep = 2;

        paperWithHoles.SetActive(false);
        notebookCover.SetActive(false);

        finishedNotebook.SetActive(true);

        CraftedInventoryManager.Instance?.UnlockNotebook();

        AudioManager.Instance?.PlayCraftComplete();

        if (completionPanel != null)
            completionPanel.SetActive(true);
    }

    //==================================================
    // PENCIL HOLDER STEP 1
    //==================================================

    public void CutBottle()
    {
        if (currentStep != 0)
            return;

        currentStep = 1;

        scissors.SetActive(false);
        dirtyBottle.SetActive(false);

        cutBottle.SetActive(true);
        sponge.SetActive(true);
    }

    //==================================================
    // PENCIL HOLDER STEP 2
    //==================================================

    public void CleanBottle()
    {
        if (currentStep != 1)
            return;

        currentStep = 2;

        sponge.SetActive(false);
        cutBottle.SetActive(false);

        cleanBottle.SetActive(true);
        coloredPaper.SetActive(true);
    }

    //==================================================
    // PENCIL HOLDER STEP 3
    //==================================================

    public void DecorateBottle()
    {
        if (currentStep != 2)
            return;

        currentStep = 3;

        coloredPaper.SetActive(false);
        cleanBottle.SetActive(false);

        finishedPencilHolder.SetActive(true);

        CraftedInventoryManager.Instance?.UnlockPencilHolder();

        AudioManager.Instance?.PlayCraftComplete();

        if (completionPanel != null)
            completionPanel.SetActive(true);
    }

    //==================================================
    // DESK ORGANIZER STEP 1
    //==================================================

    public void CutFirstMilkCarton()
    {
        if (currentStep != 0)
            return;

        currentStep = 1;

        scissors1.SetActive(false);
        milkCarton1.SetActive(false);

        scissors2.SetActive(true);
        milkCarton2.SetActive(true);
    }

    //==================================================
    // DESK ORGANIZER STEP 2
    //==================================================

    public void CutSecondMilkCarton()
    {
        if (currentStep != 1)
            return;

        currentStep = 2;

        scissors2.SetActive(false);
        milkCarton2.SetActive(false);

        glue.SetActive(true);
        twoCutCartons.SetActive(true);
    }

    //==================================================
    // DESK ORGANIZER STEP 3
    //==================================================

    public void GlueDeskOrganizer()
    {
        if (currentStep != 2)
            return;

        currentStep = 3;

        glue.SetActive(false);
        twoCutCartons.SetActive(false);

        finishedDeskOrganizer.SetActive(true);

        CraftedInventoryManager.Instance?.UnlockDeskOrganizer();

        AudioManager.Instance?.PlayCraftComplete();

        if (completionPanel != null)
            completionPanel.SetActive(true);
    }

        //==================================================
    // RESET
    //==================================================

    private void ResetCraft()
    {
        currentStep = 0;

        // Restore all draggable items to their original positions
        foreach (DraggableState state in draggableStates)
        {
            if (state.item == null)
                continue;

            RectTransform rt = state.item.GetComponent<RectTransform>();

            state.item.transform.SetParent(state.parent, false);
            state.item.transform.SetSiblingIndex(state.siblingIndex);

            rt.anchoredPosition = state.anchoredPosition;
        }

        //=========================
        // Notebook
        //=========================

        if (puncher != null)
            puncher.SetActive(true);

        if (paper != null)
            paper.SetActive(true);

        if (paperWithHoles != null)
            paperWithHoles.SetActive(false);

        if (notebookCover != null)
            notebookCover.SetActive(false);

        if (finishedNotebook != null)
            finishedNotebook.SetActive(false);

        //=========================
        // Pencil Holder
        //=========================

        if (scissors != null)
            scissors.SetActive(true);

        if (dirtyBottle != null)
            dirtyBottle.SetActive(true);

        if (cutBottle != null)
            cutBottle.SetActive(false);

        if (sponge != null)
            sponge.SetActive(false);

        if (cleanBottle != null)
            cleanBottle.SetActive(false);

        if (coloredPaper != null)
            coloredPaper.SetActive(false);

        if (finishedPencilHolder != null)
            finishedPencilHolder.SetActive(false);

        //=========================
        // Desk Organizer
        //=========================

        if (scissors1 != null)
            scissors1.SetActive(true);

        if (milkCarton1 != null)
            milkCarton1.SetActive(true);

        if (scissors2 != null)
            scissors2.SetActive(false);

        if (milkCarton2 != null)
            milkCarton2.SetActive(false);

        if (glue != null)
            glue.SetActive(false);

        if (twoCutCartons != null)
            twoCutCartons.SetActive(false);

        if (finishedDeskOrganizer != null)
            finishedDeskOrganizer.SetActive(false);

        //=========================
        // Completion
        //=========================

        if (completionPanel != null)
            completionPanel.SetActive(false);
    }

    //==================================================
    // CLOSE
    //==================================================

    public void CloseCrafting()
    {
        AudioManager.Instance?.PlayButtonClick();

        ResetCraft();

        if (craftingPopup != null)
            craftingPopup.SetActive(false);

        if (notebookPanel != null)
            notebookPanel.SetActive(false);

        if (pencilHolderPanel != null)
            pencilHolderPanel.SetActive(false);

        if (deskOrganizerPanel != null)
            deskOrganizerPanel.SetActive(false);

        // Return to the collection screen
        if (RecyclableCollectionUI.Instance != null)
        {
            RecyclableCollectionUI.Instance.collectionPanel.SetActive(true);
        }
    }
}