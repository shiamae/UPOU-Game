using TMPro;
using UnityEngine;

public class WasteInfoUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;

    public TMP_Text itemNameText;
    public TMP_Text categoryText;
    public TMP_Text decompositionText;
    public TMP_Text tipText;
    public TMP_Text descriptionText;

    private void Start()
    {
        // Hide the panel when the game starts
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    /// <summary>
    /// Displays the information of the picked-up waste item.
    /// </summary>
    public void ShowInfo(WasteItem item)
    {
        if (panel == null || item == null)
            return;

        panel.SetActive(true);

        itemNameText.text = item.itemName;
        categoryText.text = "Category: " + item.category;
        decompositionText.text = "Decomposition Time: " + item.decomposition;
        tipText.text = "3R Tip: " + item.tip;
        descriptionText.text = item.description;
    }

    /// <summary>
    /// Hides the information panel.
    /// </summary>
    public void HideInfo()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }
}