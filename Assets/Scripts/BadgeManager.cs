using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class BadgeManager : MonoBehaviour
{
    public static BadgeManager Instance;

    [System.Serializable]
    public class Badge
    {
        public string badgeName;
        [TextArea]
        public string description;

        public int requiredScore;

        public Sprite badgeIcon;

        [HideInInspector]
        public bool unlocked;
    }

    [Header("Badges")]
    public List<Badge> badges = new List<Badge>();

    [Header("Popup")]
    public GameObject badgePopupPanel;
    public Image popupBadgeIcon;
    public TMP_Text popupBadgeName;
    public TMP_Text popupDescription;

    [Header("Badge Collection")]
    public GameObject badgeCollectionPanel;

    [Header("HUD Icons")]
    public Image[] badgeSlots;

    [Header("Crosshair")]
    public GameObject crosshair;

    private bool popupShowing = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (badgePopupPanel != null)
            badgePopupPanel.SetActive(false);

        if (badgeCollectionPanel != null)
            badgeCollectionPanel.SetActive(false);

        RefreshHUD();
    }

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && !popupShowing)
        {
            ToggleBadgeCollection();
        }
    }

    //====================================================
    // Called by ScoreManager
    //====================================================

    public void CheckBadges(int currentScore)
    {
        foreach (Badge badge in badges)
        {
            if (badge.unlocked)
                continue;

            if (currentScore >= badge.requiredScore)
            {
                UnlockBadge(badge);
            }
        }
    }

    //====================================================
    // Unlock Badge
    //====================================================

    private void UnlockBadge(Badge badge)
    {
        badge.unlocked = true;

        RefreshHUD();

        ShowPopup(badge);
    }

    //====================================================
    // Popup
    //====================================================

    private void ShowPopup(Badge badge)
    {
        popupShowing = true;

        badgePopupPanel.SetActive(true);

        popupBadgeIcon.sprite = badge.badgeIcon;
        popupBadgeName.text = badge.badgeName;
        popupDescription.text = badge.description;

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (crosshair != null)
            crosshair.SetActive(false);
    }

    // Assign this to the popup X button.
    public void ClosePopup()
    {
        badgePopupPanel.SetActive(false);

        popupShowing = false;

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (crosshair != null)
            crosshair.SetActive(true);
    }

    //====================================================
    // Collection
    //====================================================

    public void ToggleBadgeCollection()
    {
        if (badgeCollectionPanel == null)
            return;

        bool open = !badgeCollectionPanel.activeSelf;

        badgeCollectionPanel.SetActive(open);

        if (open)
        {
            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (crosshair != null)
                crosshair.SetActive(false);
        }
        else
        {
            Time.timeScale = 1f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (crosshair != null)
                crosshair.SetActive(true);
        }
    }

    //====================================================
    // HUD
    //====================================================

    private void RefreshHUD()
    {
        if (badgeSlots == null)
            return;

        for (int i = 0; i < badgeSlots.Length; i++)
        {
            if (i >= badges.Count)
            {
                badgeSlots[i].enabled = false;
                continue;
            }

            badgeSlots[i].enabled = true;
            badgeSlots[i].sprite = badges[i].badgeIcon;

            Color c = badgeSlots[i].color;

            if (badges[i].unlocked)
                c.a = 1f;
            else
                c.a = 0.25f;

            badgeSlots[i].color = c;
        }
    }

    public bool IsUnlocked(int index)
    {
        if (index < 0 || index >= badges.Count)
            return false;

        return badges[index].unlocked;
    }
}
