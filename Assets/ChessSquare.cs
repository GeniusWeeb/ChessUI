using UnityEngine;
using UnityEngine.EventSystems;

public class ChessSquare : MonoBehaviour, IDropHandler
{
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null  )
        {
            {
                eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
                    this.GetComponent<RectTransform>().anchoredPosition;
                Debug.Log(this.gameObject.name);
            }

        }
        
    }
}
