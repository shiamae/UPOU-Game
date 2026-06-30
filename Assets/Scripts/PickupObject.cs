using UnityEngine;

public class PickupObject : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void PickUp(Transform holdPoint)
    {
        Debug.Log("PickUp() called on " + gameObject.name);

        rb.isKinematic = true;
        col.enabled = false;

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        Debug.Log("Drop() called on " + gameObject.name);

        transform.SetParent(null);

        rb.isKinematic = false;
        col.enabled = true;
    }
}