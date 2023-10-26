using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChessPiece : MonoBehaviour , IBeginDragHandler , IEndDragHandler , IDragHandler , IPointerDownHandler 

{

    [SerializeField] private string pName;
    [SerializeField] private Vector3 currentPosition;
    private string p = "ChessPiece";

    private RectTransform rect;
    private Image img;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        img = GetComponent<Image>();

    }

    public void Init(string name , Sprite img)
    {
        this.pName = name;
    }


    private void OnEnable()
    {
        currentPosition = this.transform.position;
    }


    #region Pointer Events

    public void OnBeginDrag(PointerEventData eventData)
    {
       
        img.raycastTarget = false;
     
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       // Debug.Log(eventData.pointerEnter.name);
        
        img.raycastTarget = true;
      
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta;
    }
    
    
    
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
    
    #endregion

   
}
