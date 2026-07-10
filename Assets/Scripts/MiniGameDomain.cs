using UnityEngine;

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
        if (IsBlockedByTrashState()) return;
        ShowLandingPage();
    }
 
    private void ShowLandingPage()
    {
        if (landingPagePanel == null)
        {
            Debug.LogWarning($"{name}: No landing page panel assigned on MiniGameDomain.");
            return;
        }
 
        landingPagePanel.SetActive(true);
        isPanelShowing = true;
 
        if (pauseGameOnEnter)
        {
            Time.timeScale = 0f;
        }
    }
 
    public void HideLandingPage()
    {
        if (landingPagePanel != null)
        {
            landingPagePanel.SetActive(false);
        }
 
        isPanelShowing = false;
 
        if (pauseGameOnEnter)
        {
            Time.timeScale = 1f;
        }
    }
 
    // Call this from a "Close" button's OnClick() in the UI if you don't want
    // it to auto-close when the player walks away.
    public void OnCloseButtonPressed()
    {
        HideLandingPage();
    }
}
