using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BadgeUI : MonoBehaviour
{
    [Header("Badge Details")]
    public Image badgeIcon;
    public TMP_Text badgeName;
    public TMP_Text badgeDescription;

    public void ShowBadge(BadgeManager.Badge badge)
    {
        if (badge == null)
            return;

        badgeIcon.sprite = badge.badgeIcon;
        badgeName.text = badge.badgeName;
        badgeDescription.text = badge.description;
    }

    public void Clear()
    {
        badgeIcon.sprite = null;
        badgeName.text = "";
        badgeDescription.text = "";
    }
}
