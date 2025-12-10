using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonLogger : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"UIButtonLogger: Click received on '{gameObject.name}'");
    }
}

