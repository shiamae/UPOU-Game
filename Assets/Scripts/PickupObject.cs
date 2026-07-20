using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PickupObject : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;

    private Renderer[] renderers;

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

    //==================================================
    // Throw Control
    //==================================================

    public bool CanThrow { get; private set; } = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        renderers = GetComponentsInChildren<Renderer>(true);
    }

    public void PickUp(Transform holdPoint)
    {
        IsHeld = true;
        CanThrow = true;

        // Save original transform
        originalParent = transform.parent;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;

        // Stop movement
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;
        col.enabled = false;

        transform.SetParent(holdPoint);

        transform.localPosition = holdPosition;
        transform.localRotation = Quaternion.Euler(holdRotation);
        transform.localScale = holdScale;

        // Ensure object is visible when picked up
        SetHeldVisible(true);

        Debug.Log($"Picked up {name}");

        if (AudioManager.Instance != null && pickupSound != null)
        {
            AudioManager.Instance.PlaySound(pickupSound);
        }
    }

    public void Release()
    {
        IsHeld = false;
        CanThrow = true;

        transform.SetParent(null);

        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;

        transform.position = currentPos;
        transform.rotation = currentRot;

        transform.localScale = originalScale;

        rb.isKinematic = false;
        col.enabled = true;

        SetHeldVisible(true);

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

        transform.SetParent(null);

        rb.isKinematic = true;
        col.enabled = false;

        SetHeldVisible(false);

        Debug.Log($"{name} hidden.");
    }

    //==================================================
    // Inventory Visibility
    //==================================================

    public void SetHeldVisible(bool visible)
    {
        if (renderers == null)
            return;

        foreach (Renderer r in renderers)
        {
            if (r != null)
                r.enabled = visible;
        }
    }

    //==================================================
    // Throw Enable / Disable
    //==================================================

    public void SetCanThrow(bool canThrow)
    {
        CanThrow = canThrow;
    }
}