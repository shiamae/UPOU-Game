using UnityEngine;

public class TrashCan : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name + " entered the trash can.");

        PickupObject bottle = other.GetComponent<PickupObject>();

        if (bottle != null)
        {
            Debug.Log("Bottle successfully thrown away!");

            Destroy(other.gameObject);
        }
    }
}