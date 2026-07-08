using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BadgeUI : MonoBehaviour
{
    [Header("Badge Details")]
    public Image badgePopup;

    public void ShowBadge(BadgeManager.Badge badge)
    {
        if (badge == null)
            return;

        badgePopup.sprite = badge.badgePopup;
    }

    public void Clear()
    {
        badgePopup.sprite = null;
    }
}
