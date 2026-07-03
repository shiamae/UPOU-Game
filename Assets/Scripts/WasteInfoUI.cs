using UnityEngine;
using UnityEngine.UI;

public class WasteInfoUI : MonoBehaviour
{
    public GameObject infoPanel;
    public Image infoImage;

    public void ShowInfo(WasteItem item)
    {
        infoPanel.SetActive(true);
        infoImage.sprite = item.infoImage;
    }

    public void HideInfo()
    {
        infoPanel.SetActive(false);
    }
}