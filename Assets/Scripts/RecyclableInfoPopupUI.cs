using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecyclableInfoPopupUI : MonoBehaviour
{
    public static RecyclableInfoPopupUI Instance;

    [Header("Popup")]
    public GameObject popupPanel;

    [Header("UI")]
    public Image itemImage;
    public TMP_Text itemTitle;
    public TMP_Text itemDescription;

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

        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    /// <summary>
    /// Opens the popup and fills it with the selected recyclable item's information.
    /// </summary>
    public void ShowInfo(RecyclableInfo info)
    {
        if (info == null)
            return;

        AudioManager.Instance?.PlayButtonClick();

        popupPanel.SetActive(true);

        if (itemImage != null)
            itemImage.sprite = info.itemImage;

        if (itemTitle != null)
            itemTitle.text = info.itemName;

        if (itemDescription != null)
            itemDescription.text = info.description;
    }

    /// <summary>
    /// Closes the information popup.
    /// </summary>
    public void ClosePopup()
    {
        AudioManager.Instance?.PlayButtonClick();

        if (popupPanel != null)
            popupPanel.SetActive(false);
    }
}