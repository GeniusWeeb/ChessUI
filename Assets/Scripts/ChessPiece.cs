using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChessPiece : MonoBehaviour , IBeginDragHandler , IEndDragHandler , IDragHandler  , IDropHandler , IPiece

{

    [SerializeField] private string pName;
    [SerializeField] private string currentPosition;
    [SerializeField] private string previousPosition;
    
    private string p = "ChessPiece";
    [SerializeField] public ChessSquare currentSquare ;
    [SerializeField] public ChessSquare previousSquare ;
    
    
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
        
    
    #endregion   
    
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
    
    public void OnDrop(PointerEventData eventData)
    {
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
        this.gameObject.SetActive(false);
        
        currentSquare.SetNewPieceOnThis(newPiece);
       
    }
    #endregion
}
