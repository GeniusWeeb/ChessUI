using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChessPiece : MonoBehaviour , IDragHandler , IDropHandler , IPointerClickHandler
{

    [SerializeField] private string pName;
    [SerializeField] private Vector3 currentPosition;
    
    public void Init(string name , Sprite img)
    {
        this.pName = name;
    }


    private void OnEnable()
    {
        currentPosition = this.transform.position;
    }


    #region Pointer Events
    
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
    #endregion    
}
