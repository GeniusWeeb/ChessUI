using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChessPiece : MonoBehaviour , IBeginDragHandler , IEndDragHandler , IDragHandler  , IDropHandler , IPiece

{

    [SerializeField] private string pName;
    [SerializeField] private string currentPosition;
    [SerializeField] private string previousPosition;
    
    private string p = "ChessPiece";
    //CEllS TRACKER
    [SerializeField] public ChessSquare currentSquare ; 
    [SerializeField] public ChessSquare previousSquare ;
    
    //SNAP POSITION TRACKERS
    public Vector2 currentRectTransform ;
    public Vector2 oldRectTransform ;

    public bool myTurn = false;
    public int pCode;
    public int pColor;
    
    private RectTransform rect;
    private Image img;

    public string GetCurrentPosition => currentPosition;
    public string GetOldPosition => previousPosition;
    
    
    
    
    
    #region UnityMethods
        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            img = GetComponent<Image>();

        }

        public void Init(string name , Sprite img)
        {
            this.pName = name;
        }

        public ChessPiece()
        {
            
        }
        
    
    #endregion   
    
    #region Pointer Events

    public void OnBeginDrag(PointerEventData eventData)
    {   
       
        if (!myTurn) return; 
        img.raycastTarget = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {   
        //Debug.Log("Drag finish maybe");
        Event.ResetCellColor.Invoke();
        ChessManager.Instance.requestedData = false;
        
        //DISABLE UI INDICATOR HERE FOR CELLS
        img.raycastTarget = true;
      
    }

    public void OnDrag(PointerEventData eventData)
    {
       
        if (!ChessManager.Instance.requestedData)
        {
            ChessManager.Instance.RequestPossibleCellDataForThisIndex(currentSquare.currentIndex);
            ChessManager.Instance.requestedData = true;
        }

        if (!myTurn) return; 
        rect.anchoredPosition += eventData.delta;
    }
    
    public void OnDrop(PointerEventData eventData)
    {     
        
        if (!eventData.pointerDrag.GetComponent<ChessPiece>().myTurn ||
            pColor == eventData.pointerDrag.GetComponent<ChessPiece>().pColor)
        {
            eventData.pointerDrag.GetComponent<ChessPiece>().GetComponent<RectTransform>().anchoredPosition =
                eventData.pointerDrag.GetComponent<ChessPiece>().currentRectTransform;
        }
        

        Debug.Log(eventData.pointerDrag.name);
        
        if (eventData.pointerDrag.GetComponent<IPiece>() != null)
        { 
            Captured(eventData.pointerDrag);
        }

    }

    public void SetCurrentPosition(string value)
    {
        currentPosition = value;
    }

    public void SetOldPosition(string value)
    {
        previousPosition = value;

    }

    public void Captured(GameObject newPiece)
    {
        if (pColor == newPiece.GetComponent<ChessPiece>().pColor)
            return;
        
        this.gameObject.SetActive(false);
        
        //CAPTURE MECHANIC PENDING
            
        //currentSquare.SetNewPieceOnThis(newPiece);
        ChessManager.Instance.SetNewPieceOnThis(newPiece ,currentSquare.gameObject);
       
    }
    #endregion

    
}
