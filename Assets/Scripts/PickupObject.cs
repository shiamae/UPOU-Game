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

    public bool IsHeld { get; private set; }

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
    }

    public void Dispose()
    {
        Debug.Log($"{name} disposed.");

        Destroy(gameObject);
    }
}