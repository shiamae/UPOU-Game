using UnityEngine;

public class WasteItem : MonoBehaviour
{
    [Header("Basic Information")]
    public string itemName;

    public string category;

    public string decomposition;

    [TextArea(2, 4)]
    public string tip;

    [TextArea(3, 6)]
    public string description;
}