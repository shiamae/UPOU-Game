using System.Collections;
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

    [Header("Badge Notification")]
    public GameObject notificationPanel;
    public Image notificationIcon;
    public TMP_Text notificationTitle;
    public TMP_Text notificationText;

    [Tooltip("How long the notification stays on screen.")]
    public float notificationDuration = 3f;

    [Header("Badge Collection")]
    public GameObject badgeCollectionPanel;

    [Header("HUD Badge Icons")]
    public Image[] badgeSlots;

    private Coroutine notificationRoutine;

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

        if (notificationPanel != null)
            notificationPanel.SetActive(false);

        if (badgeCollectionPanel != null)
            badgeCollectionPanel.SetActive(false);

        RefreshHUD();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
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

        ShowNotification(badge);

        Debug.Log("Unlocked Badge: " + badge.badgeName);
    }

    //====================================================
    // Notification
    //====================================================

    private void ShowNotification(Badge badge)
    {
        if (notificationRoutine != null)
            StopCoroutine(notificationRoutine);

        notificationPanel.SetActive(true);

        notificationIcon.sprite = badge.badgeIcon;

        notificationTitle.text = $"<b>{badge.badgeName}</b>" + $"Earned!";

        notificationText.text =
            $"Congrats! You earned the badge " +
            $"<b>{badge.badgeName}</b> " +
            $"for reaching {badge.requiredScore} points.";

        notificationRoutine = StartCoroutine(HideNotification());
    }

    private IEnumerator HideNotification()
    {
        yield return new WaitForSeconds(notificationDuration);

        notificationPanel.SetActive(false);
    }

    //====================================================
    // Badge Collection
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
        }
        else
        {
            Time.timeScale = 1f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    //====================================================
    // HUD Icons
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

            Color color = badgeSlots[i].color;

            color.a = badges[i].unlocked ? 1f : 0.25f;

            badgeSlots[i].color = color;
        }
    }

    //====================================================
    // Public Helpers
    //====================================================

    public bool IsUnlocked(int index)
    {
        if (index < 0 || index >= badges.Count)
            return false;

        return badges[index].unlocked;
    }

    public Badge GetBadge(int index)
    {
        if (index < 0 || index >= badges.Count)
            return null;

        return badges[index];
    }
}