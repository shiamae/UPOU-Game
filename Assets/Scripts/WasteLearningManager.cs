using System.Collections.Generic;
using UnityEngine;

public class WasteLearningManager : MonoBehaviour
{
    public static WasteLearningManager Instance;
    // Stores every waste type the player has already learned.
    private readonly HashSet<WasteType> learnedWaste = new HashSet<WasteType>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool HasLearned(WasteType wasteType)
    {
        return learnedWaste.Contains(wasteType);
    }

    public void Learn(WasteType wasteType)
    {
        if (!learnedWaste.Contains(wasteType))
        {
            learnedWaste.Add(wasteType);
            Debug.Log($"Learned waste: {wasteType}");
        }
    }

    public void ResetLearning()
    {
        learnedWaste.Clear();
    }
}