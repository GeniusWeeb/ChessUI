using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChessSquare : MonoBehaviour, IDropHandler 
{

    public ChessPiece currentP;
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null  )
        {   
            //Force the pieces previous square to be empty now that it is here
            if (eventData.pointerDrag.GetComponent<ChessPiece>().currentSquare != null) {
                eventData.pointerDrag.GetComponent<ChessPiece>().currentSquare.GetComponent<ChessSquare>().currentP = null;
            }
            SetNewPieceOnThis(eventData.pointerDrag);
        }
    }

    public void SetNewPieceOnThis(GameObject p)
    {
        
        currentP = p.GetComponent<ChessPiece>();
        currentP.GetComponent<RectTransform>().anchoredPosition = 
            this.GetComponent<RectTransform>().anchoredPosition;
        currentP.currentSquare = this.gameObject.GetComponent<ChessSquare>();
        ShowAndPerformNotation(currentP.name , this.gameObject.name);
    }

    private void ShowAndPerformNotation(string name , string moveSquare)
    {
        Debug.Log(name+""+ moveSquare);
        Event.MoveMade.Invoke();
    }

}
