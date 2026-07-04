using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PickupObject : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;

    private Transform originalParent;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    [Header("Hold Settings")]
    public Vector3 holdPosition = new Vector3(0.35f, -0.15f, 0.55f);
    public Vector3 holdRotation = new Vector3(0f, 0f, 0f);
    public Vector3 holdScale = Vector3.one;

    [Header("Audio")]
    public AudioClip pickupSound;
    public AudioClip dropSound;

    public bool IsHeld { get; private set; }
    public bool IsDisposed { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void PickUp(Transform holdPoint)
    {
        IsHeld = true;

        // Save original transform
        originalParent = transform.parent;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;

        // Stop all movement BEFORE making the Rigidbody kinematic
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Disable physics
        rb.isKinematic = true;

        // Disable collider while holding
        col.enabled = false;

        // Parent to the HoldPoint
        transform.SetParent(holdPoint);

        // Apply this prefab's hold settings
        transform.localPosition = holdPosition;
        transform.localRotation = Quaternion.Euler(holdRotation);
        transform.localScale = holdScale;

        Debug.Log($"Picked up {name}");

        Debug.Log("Trying to play pickup sound.");

        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager.Instance is NULL!");
        }
        else if (pickupSound == null)
        {
            Debug.LogError("Pickup Sound is NULL!");
        }
        else
        {
            Debug.Log("Playing: " + pickupSound.name);
            AudioManager.Instance.PlaySound(pickupSound);
        }

        if (AudioManager.Instance != null && pickupSound != null)
        {
            AudioManager.Instance.PlaySound(pickupSound);
        }
    }

    public void Release()
    {
        IsHeld = false;

        transform.SetParent(null);

        // Keep the current world transform
        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;

        transform.position = currentPos;
        transform.rotation = currentRot;

        transform.localScale = originalScale;

        rb.isKinematic = false;
        col.enabled = true;

        Debug.Log($"Released {name}");

        if (AudioManager.Instance != null && dropSound != null)
        {
            AudioManager.Instance.PlaySound(dropSound);
        }
    }

    public void Dispose()
    {
        IsDisposed = true;

        Debug.Log($"{name} disposed.");

        Destroy(gameObject);
    }

    public void Hide()
    {
        IsHeld = false;

        // Detach from the player's hand
        transform.SetParent(null);

        // Disable physics
        rb.isKinematic = true;
        col.enabled = false;

        // Hide all renderers on this object
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }

        Debug.Log($"{name} hidden.");
    }
}