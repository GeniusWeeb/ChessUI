using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChessPiece : MonoBehaviour , IDragHandler , IDropHandler , IPointerClickHandler
{

    [SerializeField] private string name;
    [SerializeField] private string currentPosition;
    
    public void Init(string name , Sprite img)
    {
        this.name = name;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        //select and move
        
    }

    public void OnDrop(PointerEventData eventData)
    {
       // Check if the object can be safely dropped here
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
