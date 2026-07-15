using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [System.Serializable]
    public class Quest
    {
        [Header("Quest")]
        public WasteType wasteType;
        public int requiredAmount = 1;

        [Header("Reward")]
        public RecyclableReward reward;

        [Header("Quest Image")]
        public Image questImage;
        public Sprite hiddenSprite;
        public Sprite revealedSprite;

        [Header("Checkbox")]
        public Image checkboxImage;
        public Sprite uncheckedSprite;
        public Sprite checkedSprite;

        [HideInInspector] public bool discovered;
        [HideInInspector] public bool completed;
        [HideInInspector] public int currentAmount;
    }

    [Header("Quests")]
    public List<Quest> quests = new List<Quest>();

    private Dictionary<WasteType, Quest> questLookup;

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

        questLookup = new Dictionary<WasteType, Quest>();

        foreach (Quest quest in quests)
        {
            if (!questLookup.ContainsKey(quest.wasteType))
                questLookup.Add(quest.wasteType, quest);

            quest.discovered = false;
            quest.completed = false;
            quest.currentAmount = 0;

            // Initialize UI
            if (quest.questImage != null && quest.hiddenSprite != null)
                quest.questImage.sprite = quest.hiddenSprite;

            if (quest.checkboxImage != null && quest.uncheckedSprite != null)
                quest.checkboxImage.sprite = quest.uncheckedSprite;
        }
    }

    /// <summary>
    /// Called ONLY after the player correctly disposes of recyclable waste.
    /// </summary>
    public void RegisterRecycle(WasteItem waste)
    {
        if (waste == null)
            return;

        if (waste.wasteType == WasteType.None)
            return;

        if (!questLookup.TryGetValue(waste.wasteType, out Quest quest))
            return;

        // Already completed
        if (quest.completed)
            return;

        // Reveal the quest after the player correctly disposes
        // of this waste for the first time.
        if (!quest.discovered)
        {
            quest.discovered = true;

            if (quest.questImage != null &&
                quest.revealedSprite != null)
            {
                quest.questImage.sprite = quest.revealedSprite;
            }
        }

        // Increase progress
        quest.currentAmount++;

        Debug.Log($"{quest.wasteType}: {quest.currentAmount}/{quest.requiredAmount}");

        if (quest.currentAmount >= quest.requiredAmount)
        {
            CompleteQuest(quest);
        }
    }

    private void CompleteQuest(Quest quest)
    {
        if (quest.completed)
            return;

        quest.completed = true;

        if (quest.checkboxImage != null &&
            quest.checkedSprite != null)
        {
            quest.checkboxImage.sprite = quest.checkedSprite;
        }

        Debug.Log($"Quest Completed: {quest.wasteType}");

        // Unlock the corresponding recyclable item
        if (RecyclableCollectionUI.Instance != null)
        {
            switch (quest.reward)
            {
                case RecyclableReward.Notebook:
                    RecyclableCollectionUI.Instance.UnlockNotebook();
                    break;

                case RecyclableReward.PencilHolder:
                    RecyclableCollectionUI.Instance.UnlockPencilHolder();
                    break;

                case RecyclableReward.DeskOrganizer:
                    RecyclableCollectionUI.Instance.UnlockDeskOrganizer();
                    break;
            }
        }

        // Optional unlock sound
        AudioManager.Instance?.PlayButtonClick();
    }

    //==================================================
    // Helper Functions
    //==================================================

    public bool IsQuestCompleted(WasteType type)
    {
        if (!questLookup.TryGetValue(type, out Quest quest))
            return false;

        return quest.completed;
    }

    public int GetProgress(WasteType type)
    {
        if (!questLookup.TryGetValue(type, out Quest quest))
            return 0;

        return quest.currentAmount;
    }
}