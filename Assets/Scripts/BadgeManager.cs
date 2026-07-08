using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BadgeManager : MonoBehaviour
{
    public static BadgeManager Instance;

    [System.Serializable]
    public class Badge
    {
        [Header("Unlock")]
        public int requiredScore;

        [Header("Notification Popup Image")]
        public Sprite badgePopup;

        [Header("HUD Badge Icon")]
        public Sprite badgeIcon;

        [HideInInspector]
        public bool unlocked;
    }

    [Header("Badges")]
    public List<Badge> badges = new List<Badge>();

    [Header("Notification")]
    public GameObject notificationPanel;
    public Image notificationImage;
    public float notificationDuration = 3f;

    [Header("Badge Collection")]
    public GameObject badgeCollectionPanel;

    [Header("HUD Badge Icons")]
    public Image[] badgeSlots;

    private Coroutine notificationRoutine;

    // Badge waiting to be shown after the education popup closes
    private Badge pendingBadge;

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

    //==================================================
    // Called by ScoreManager
    //==================================================

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

    //==================================================
    // Unlock Badge
    //==================================================

    private void UnlockBadge(Badge badge)
    {
        badge.unlocked = true;

        RefreshHUD();

        // Save the badge for later.
        // It will appear after the education popup closes.
        pendingBadge = badge;

        Debug.Log($"Unlocked badge ({badge.requiredScore} points)");
    }

    //==================================================
    // Called by GameUIManager
    //==================================================

    public void ShowPendingBadge()
    {
        if (pendingBadge == null)
            return;

        ShowNotification(pendingBadge);

        pendingBadge = null;
    }

    //==================================================
    // Notification
    //==================================================

    private void ShowNotification(Badge badge)
    {
        if (notificationPanel == null || notificationImage == null)
            return;

        if (notificationRoutine != null)
            StopCoroutine(notificationRoutine);

        notificationImage.sprite = badge.badgePopup;

        notificationPanel.SetActive(true);

        // Play badge unlock sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBadgeUnlock();
        }

        notificationRoutine = StartCoroutine(HideNotification());
    }

    private IEnumerator HideNotification()
    {
        yield return new WaitForSeconds(notificationDuration);

        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }

    //==================================================
    // Badge Collection
    //==================================================

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

    //==================================================
    // HUD
    //==================================================

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
            c.a = badges[i].unlocked ? 1f : 0.25f;
            badgeSlots[i].color = c;
        }
    }

    //==================================================
    // Helpers
    //==================================================

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