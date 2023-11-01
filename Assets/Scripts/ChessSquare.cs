using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChessSquare : MonoBehaviour, IDropHandler
{
    public TextMeshProUGUI squreText;
    public ChessPiece currentP;
    public int currentIndex;
    public bool captured;
    private Image thisCellImg;
   [SerializeField ]  private Color defaultColor;
    [SerializeField] private bool iGotHighlighted;
    
    public void OnDrop(PointerEventData eventData)
    {   
        
        Debug.Log("Dropped on this");
        if (!eventData.pointerDrag.GetComponent<ChessPiece>().myTurn)
            return;
        
        if (eventData.pointerDrag != null    )
        {   
            //VALIDATION NEEDED!   //IF MOVE NOT ALLOWED -> RETURN -> GUARD CLAUSE 
            //Force the pieces previous square to be empty now that it is here
            if (eventData.pointerDrag.GetComponent<ChessPiece>().currentSquare != null) {
                eventData.pointerDrag.GetComponent<ChessPiece>().currentSquare.GetComponent<ChessSquare>().currentP = null;
            }
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = 
                this.GetComponent<RectTransform>().anchoredPosition;

            currentP = eventData.pointerDrag.GetComponent<ChessPiece>();

            if (currentP.currentRectTransform == this.GetComponent<RectTransform>().anchoredPosition)
                return;

    
            string moveHistory = currentP.GetCurrentPosition+""+this.gameObject.name;
            
            // Control should not return here again to this chess square once the move has been sent to validate
            //Control shifts completely to chess manager who will validate and confirm the move on UI.
            //LEARN : I MADE THE CONTROL MESSED UP TRYING TO PLUG THE RESULT IN HERE 
            ConfirMoveMade(currentP.name, moveHistory);
            //just send the data -> piece and the square object
            ChessManager.Instance. SetNewPieceOnThis(eventData.pointerDrag , this.gameObject);
            
        }
    }


    public void SetDefaultColor(Color color)
    {
        defaultColor = color;
    }

    private void  ConfirMoveMade(string name , string moveSquare)
    {
        ChessManager.Instance.SendMoveMadeToEngine(name ,moveSquare);
     
    }

    
    
    //THIS RECEIVES THE UI CELL EVENT - PER SQUARE BASIS AND LIGHTS ITSELF UP
    private void ShowCell(HashSet<int> data)
    {
       
        if (data.Contains(currentIndex))
        {
            iGotHighlighted = true;
            thisCellImg.color =  ChessManager.Instance.GetChessConfig.highlightColor;
            Color currentColor = thisCellImg.color;
            currentColor.a = 55f;
            thisCellImg.color = currentColor;
        }
        
    }

    public void ResetColorOnDrop()
    {
        if (iGotHighlighted)
        {
            thisCellImg.color = defaultColor;
        }

    }

    #region UnityMethods

    private void Awake()
    {
        thisCellImg = GetComponent<Image>();
       
    }

    public void OnEnable()
        {
            Event<HashSet<int>>.GameUIShowCell += ShowCell;
            Event.ResetCellColor += ResetColorOnDrop;

        }

        public void OnDisable()
        {
            Event<HashSet<int>>.GameUIShowCell -= ShowCell;
            Event.ResetCellColor -= ResetColorOnDrop;

        }

    #endregion
  

}
