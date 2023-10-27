using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChessSquare : MonoBehaviour, IDropHandler 
{

    public ChessPiece currentP;
    public bool captured; 
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null  )
        {   
           //VALIDATION NEEDED!   //IF MOVE NOT ALLOWED -> RETURN -> GUARD CLAUSE 
            //Force the pieces previous square to be empty now that it is here
            if (eventData.pointerDrag.GetComponent<ChessPiece>().currentSquare != null) {
                eventData.pointerDrag.GetComponent<ChessPiece>().currentSquare.GetComponent<ChessSquare>().currentP = null;
            }
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = 
                this.GetComponent<RectTransform>().anchoredPosition;

            currentP = eventData.pointerDrag.GetComponent<ChessPiece>();
            
            //  if(currentP.currentRectTransform !=  this.GetComponent<RectTransform>().anchoredPosition) {
            //     currentP.oldRectTransform = currentP.currentRectTransform;
            //     currentP.currentRectTransform = currentP.GetComponent<RectTransform>().anchoredPosition;
            //     return;
            // }

            if (currentP.currentRectTransform == this.GetComponent<RectTransform>().anchoredPosition)
                return;

            //  
            // MOVE FALSE? RETURN 
            string moveHistory = currentP.GetCurrentPosition+""+this.gameObject.name;
           
         
            // Control should not return here again to this chess square once the move has been sent to validate
            //Control shifts completely to chess manager who will validate and confirm the move on UI.
            
            //LEARN : I MADE THE CONTROL MESSED UP TRYING TO PLUG THE RESULT IN HERE 
            ConfirMoveMade(currentP.name, moveHistory);
            //just send the data -> piece and the square object
            ChessManager.Instance. SetNewPieceOnThis(eventData.pointerDrag , this.gameObject);
            
        }
    }



    private void  ConfirMoveMade(string name , string moveSquare)
    {
        ChessManager.Instance.SendMoveMadeToEngine(name ,moveSquare);
       // Event.MoveMade.Invoke(); // counter -> can be moved
    }


  

}
