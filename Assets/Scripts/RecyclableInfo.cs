using UnityEngine;

[CreateAssetMenu(fileName = "New Recyclable Info", menuName = "Recyclables/Recyclable Info")]
public class RecyclableInfo : ScriptableObject
{
    [Header("Basic Information")]
    public string itemName;

    [Header("Display")]
    public Sprite itemImage;

    [Header("Description")]
    [TextArea(5, 10)]
    public string description;
}