using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class MiniGameDomain : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("The landing page panel/canvas for the mini-game. Should start inactive in the scene.")]
    [SerializeField] private GameObject landingPagePanel;
 
    [Header("Behavior")]
    [Tooltip("Tag used to identify the player.")]
    [SerializeField] private string playerTag = "Player";
 
    [Tooltip("Pause the game while the landing page is open.")]
    [SerializeField] private bool pauseGameOnEnter = true;
 
    [Tooltip("If true, panel closes automatically when the player leaves the domain.")]
    [SerializeField] private bool closeOnExit = true;
 
    [Tooltip("How long after disposing trash before the panel is allowed to show again.")]
    [SerializeField] private float disposeCooldown = 1.5f;
 
    private bool isPlayerInside = false;
    private bool isPanelShowing = false;
    private static int activeLandingPageCount = 0;
    private static MiniGameDomain hotkeyOwner;
    private static bool manuallyHiddenByHotkey = false;
    private static readonly List<MiniGameDomain> domains = new List<MiniGameDomain>();

    public static bool IsAnyLandingPageShowing => activeLandingPageCount > 0;

    private void OnEnable()
    {
        if (!domains.Contains(this))
            domains.Add(this);

        if (hotkeyOwner == null && landingPagePanel != null)
            hotkeyOwner = this;
    }
 
    private void Reset()
    {
        // Ensure the collider is set up as a trigger by default when the script is added.
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }
 
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
 
        isPlayerInside = true;
        TryShowLandingPage();
    }
 
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
 
        if (manuallyHiddenByHotkey)
            return;

        // Re-check every frame the player stays inside, in case they were
        // holding/disposing trash on entry and only become "free" partway through.
        if (!isPanelShowing)
        {
            TryShowLandingPage();
        }
    }
 
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
 
        isPlayerInside = false;
 
        if (closeOnExit)
        {
            HideLandingPage();
        }
    }

    private void Update()
    {
        if (hotkeyOwner != this)
            return;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (keyboard.hKey.wasPressedThisFrame)
        {
            ToggleLandingPage();
        }
        else if (keyboard.escapeKey.wasPressedThisFrame && IsAnyLandingPageShowing)
        {
            HideAllLandingPages();
            manuallyHiddenByHotkey = true;
        }
    }
 
    private bool IsBlockedByTrashState()
    {
        // Block if the player already has points on the board.
        if (ScoreManager.Instance != null && ScoreManager.Instance.Score != 0)
        {
            return true;
        }
 
        if (PlayerPickup.Instance == null) return false;
 
        return PlayerPickup.Instance.IsHoldingObject ||
               PlayerPickup.Instance.RecentlyDisposedTrash(disposeCooldown);
    }
 
    private void TryShowLandingPage()
    {
        if (manuallyHiddenByHotkey) return;
        if (IsBlockedByTrashState()) return;
        ShowLandingPage();
    }
 
    private void ShowLandingPage()
    {
        if (isPanelShowing)
            return;

        if (landingPagePanel == null)
        {
            Debug.LogWarning($"{name}: No landing page panel assigned on MiniGameDomain.");
            return;
        }
 
        landingPagePanel.SetActive(true);
        isPanelShowing = true;
        activeLandingPageCount++;
 
        if (pauseGameOnEnter)
        {
            Time.timeScale = 0f;
        }
    }
 
    public void HideLandingPage()
    {
        if (!isPanelShowing)
            return;

        if (landingPagePanel != null)
        {
            landingPagePanel.SetActive(false);
        }
 
        isPanelShowing = false;
        activeLandingPageCount = Mathf.Max(0, activeLandingPageCount - 1);
 
        if (pauseGameOnEnter)
        {
            Time.timeScale = 1f;
        }
    }

    public void ToggleLandingPage()
    {
        if (IsAnyLandingPageShowing)
        {
            HideAllLandingPages();
            manuallyHiddenByHotkey = true;
        }
        else
        {
            manuallyHiddenByHotkey = false;
            ShowLandingPage();
        }
    }

    private void OnDisable()
    {
        domains.Remove(this);

        if (hotkeyOwner == this)
            hotkeyOwner = null;

        if (!isPanelShowing)
            return;

        isPanelShowing = false;
        activeLandingPageCount = Mathf.Max(0, activeLandingPageCount - 1);

        if (pauseGameOnEnter)
        {
            Time.timeScale = 1f;
        }
    }
 
    // Call this from a "Close" button's OnClick() in the UI if you don't want
    // it to auto-close when the player walks away.
    public void OnCloseButtonPressed()
    {
        HideAllLandingPages();
        manuallyHiddenByHotkey = true;
    }

    private static void HideAllLandingPages()
    {
        for (int i = 0; i < domains.Count; i++)
        {
            if (domains[i] != null)
                domains[i].HideLandingPage();
        }

        activeLandingPageCount = 0;
    }
}
