using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles teleportation to the nearest trash can using shortcut keys.
/// </summary>
public class TrashCanTeleporter : MonoBehaviour
{
    public static TrashCanTeleporter Instance;

    [Header("Teleportation Settings")]
    public float teleportHeight = 0.5f; // Height offset above ground where player spawns
    public float teleportDistance = 5f; // Distance in front of trash can

    private TrashCan[] allTrashCans;
    private CharacterController characterController;

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

        // Cache all trash cans in the scene
        allTrashCans = FindObjectsByType<TrashCan>(FindObjectsSortMode.None);
        characterController = GetComponent<CharacterController>();

        Debug.Log($"[TrashCanTeleporter] Found {allTrashCans.Length} trash cans in the scene.");
    }

    private void Update()
    {
        // Check for teleport shortcut keys (T for teleport to nearest, 1-5 for specific cans)
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            TeleportToNearestTrashCan();
        }

        // Number keys for specific trash cans
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            TeleportToTrashCan(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            TeleportToTrashCan(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            TeleportToTrashCan(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            TeleportToTrashCan(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
            TeleportToTrashCan(4);
    }

    /// <summary>
    /// Teleports the player to the nearest trash can.
    /// </summary>
    public void TeleportToNearestTrashCan()
    {
        if (allTrashCans.Length == 0)
        {
            Debug.LogWarning("[TrashCanTeleporter] No trash cans found in the scene.");
            return;
        }

        TrashCan nearestTrashCan = FindNearestTrashCan();

        if (nearestTrashCan != null)
        {
            TeleportToPosition(nearestTrashCan.transform.position);
            ShowTeleportNotification(nearestTrashCan);
        }
    }

    /// <summary>
    /// Teleports the player to a specific trash can by index.
    /// </summary>
    public void TeleportToTrashCan(int index)
    {
        if (index < 0 || index >= allTrashCans.Length)
        {
            Debug.LogWarning($"[TrashCanTeleporter] Trash can index {index} out of range. Available: {allTrashCans.Length}");
            return;
        }

        TrashCan targetTrashCan = allTrashCans[index];

        if (targetTrashCan != null)
        {
            TeleportToPosition(targetTrashCan.transform.position);
            ShowTeleportNotification(targetTrashCan);
        }
    }

    /// <summary>
    /// Finds the nearest trash can to the player.
    /// </summary>
    private TrashCan FindNearestTrashCan()
    {
        TrashCan nearestTrashCan = null;
        float nearestDistance = Mathf.Infinity;

        foreach (TrashCan trashCan in allTrashCans)
        {
            if (trashCan == null)
                continue;

            float distance = Vector3.Distance(transform.position, trashCan.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTrashCan = trashCan;
            }
        }

        return nearestTrashCan;
    }

    /// <summary>
    /// Teleports the player to the specified position.
    /// </summary>
    private void TeleportToPosition(Vector3 targetPosition)
    {
        // Disable the character controller temporarily
        if (characterController != null)
            characterController.enabled = false;

        // Calculate the teleport position (above the target position)
        Vector3 teleportPos = targetPosition + Vector3.up * teleportHeight;

        // Set the player's position
        transform.position = teleportPos;

        // Re-enable the character controller
        if (characterController != null)
            characterController.enabled = true;

        Debug.Log($"[TrashCanTeleporter] Teleported to position: {teleportPos}");
    }

    /// <summary>
    /// Shows a notification that the player has been teleported.
    /// </summary>
    private void ShowTeleportNotification(TrashCan trashCan)
    {
        Debug.Log($"[TrashCanTeleporter] Teleported to {trashCan.GetDisplayName()}");

        // You can expand this to show a UI notification
        // For now, we just log it and show the trash bin info
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowTrashBinInfo(trashCan);
        }
    }
}
