using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Displays help information about available shortcuts and controls.
/// Can be toggled with the H key.
/// </summary>
public class ShortcutHelpUI : MonoBehaviour
{
    public static ShortcutHelpUI Instance;

    [Header("Help Panel")]
    public GameObject helpPanel;
    public Text helpText;

    private bool isHelpVisible = false;

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

        if (helpPanel != null)
            helpPanel.SetActive(false);
    }

    private void Update()
    {
        // Toggle help with H key
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            ToggleHelp();
        }

        // Close help with ESC key
        if (Keyboard.current.escapeKey.wasPressedThisFrame && isHelpVisible)
        {
            HideHelp();
        }
    }

    public void ToggleHelp()
    {
        if (isHelpVisible)
            HideHelp();
        else
            ShowHelp();
    }

    public void ShowHelp()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(true);
            isHelpVisible = true;

            if (helpText != null)
            {
                helpText.text = GetHelpText();
            }

            Debug.Log("[ShortcutHelpUI] Help panel shown. Press H to hide.");
        }
    }

    public void HideHelp()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
            isHelpVisible = false;
            Debug.Log("[ShortcutHelpUI] Help panel hidden.");
        }
    }

    private string GetHelpText()
    {
        return @"<b>KEYBOARD SHORTCUTS</b>

<b>Movement & Interaction:</b>
• W/A/S/D - Move forward/left/backward/right
• SPACE - Jump
• SHIFT - Run
• R - Crouch
• LEFT CLICK - Pick up / Dispose waste

<b>Trash Can Hints & Teleportation:</b>
• HOVER - View trash can information & hints
• T - Teleport to nearest trash can
• 1-5 - Teleport to trash can #1-5
• H - Show/Hide this help menu
• ESC - Close this help menu

<b>Tips:</b>
• Read the hints carefully - they tell you what each bin accepts without spoiling the answer!
• Use T to quickly navigate to the nearest trash can
• Look for the category hints (Recyclable, Biodegradable, etc.)
• Check the examples for clues about what belongs in each bin";
    }
}
