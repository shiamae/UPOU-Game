using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class CollectionButton : MonoBehaviour
{
    [Header("Information to Display")]
    public RecyclableInfo recyclableInfo;

    public void OpenInfo()
    {
        if (recyclableInfo == null)
        {
            Debug.LogWarning($"{name} has no RecyclableInfo assigned.");
            return;
        }

        if (RecyclableInfoPopupUI.Instance != null)
        {
            RecyclableInfoPopupUI.Instance.ShowInfo(recyclableInfo);
        }
    }
}