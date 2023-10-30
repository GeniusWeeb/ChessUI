
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;


public class ChessManager :MonoBehaviour
    {
        #region Variables
                
            [SerializeField] private List<string> moveHistory;
            [SerializeField] private List<MoveTracker> trackMove;
            
            [SerializeField] private ChessConfig config;
            [SerializeField] private Transform parentTransform;
            [SerializeField] private GameObject piecePrefab;
            [SerializeField] private int  xOffset;
            [SerializeField] private int  yOffset;
            [SerializeField] private int  size;
            
            [Header("For testing positions")]
            [SerializeField] private List<PandIndex> indexs;
            
            [Header("Move Counter")]
            [SerializeField] private int moveCounter ;

            [Header("Turn to Move")] 
            [SerializeField]
            private  MoveTurn turn;
            [SerializeField] private int[] internalBoard = new int[64];
            //Force this info to come from the engin and when you decipher the FEN mapper
           [SerializeField] private TextMeshProUGUI moveCounterTxt;
           [SerializeField] private TextMeshProUGUI moveTurnTxt;
           private ChessPiece cp;

           [SerializeField] private GameObject pieceThatMadeMove;
           [SerializeField] private GameObject squareThatPieceMovedTo;
           
           [Header("Chessboard and Piece holders")] [SerializeField]
           private Transform chessBoardHolder;

        
           
       
       #endregion
       
        
        public static ChessManager Instance
        { get; private set;
        }
        
        private void MoveMade()
        {
            //Come From Engine as well
            moveCounter += 1;
            moveCounterTxt.text = moveCounter.ToString();
        }

        
        //REQUIRES MOVE PROTOCOL
        public void SendMoveMadeToEngine(string pieceName , string squareName)
        {
            bool canMakeMove = false;
            string move = pieceName + "-" + squareName;
            moveHistory.Add(move);
            var colorCode = (int)turn;
            DataProtocol moveData = new DataProtocol(ProtocolTypes.MOVE.ToString() , move , colorCode.ToString()  );
            Connection.Instance.SendMessage(moveData);
            Debug.Log( $"<color=red> {move} </color>");
        }
   
        private void ValidationResult( bool canMove)
        {

            if (canMove)
            {   
                Debug.Log("Move can take place");
                PerformMoveFinal();
            }
            else
            {
                Debug.Log("Move cant take place");
                ResetPiecePosition();
            }
        }

        private void ResetPiecePosition()
        {
            pieceThatMadeMove.GetComponent<RectTransform>().anchoredPosition =
                pieceThatMadeMove.GetComponent<ChessPiece>().currentRectTransform;
            pieceThatMadeMove = null;
            squareThatPieceMovedTo = null;
        }

        //Just set temporary
        public void SetNewPieceOnThis(GameObject p , GameObject newSquare)
        {
            pieceThatMadeMove = p;
            squareThatPieceMovedTo = newSquare;
        }
        private void PerformMoveFinal()
        {
            GameObject p = pieceThatMadeMove;
            GameObject newSquare = squareThatPieceMovedTo;
            // IF MOVE IS NOT CORRECT , SNAP IT BACK TO OLD POSITION AND JUST RETURN
            //CONFIRMATION THAT THE MOVE IS SUCCESSFUL
            ChessPiece  currentP = newSquare.GetComponent<ChessSquare>().currentP;
            currentP = p.GetComponent<ChessPiece>();
            //UPDATE TRANSFORMS ONCE MOVE HAS BEE MADE
            currentP.oldRectTransform = currentP.currentRectTransform;
            currentP.currentRectTransform = newSquare.GetComponent<RectTransform>().anchoredPosition;
            currentP.previousSquare = currentP.currentSquare;
            currentP.currentSquare = newSquare.gameObject.GetComponent<ChessSquare>();
            currentP.SetOldPosition(currentP.GetCurrentPosition);
            currentP.SetCurrentPosition(newSquare.gameObject.name);
            
            
            trackMove.Add( new MoveTracker( currentP,currentP.previousSquare ,currentP.currentSquare));
            MoveMade();

        }
    
        
        //ui interaction
        public void UndoMove()
        {
            #region UI BASED UNDO -> SEND THE BOARD STATE TO THE ENGINE
                    if (trackMove.Count == 0) //Empty array return
                        return;
                    //Most recent move , right now we are only doing  1 move at a time.
                    //next step -> based on length, we need to undo 2 moves, 1 black and 1 white
                    var moveData = trackMove[^1];
                    moveData.piece.currentSquare = moveData.from;
                    moveData.piece.previousSquare = moveData.to; // Note : can be changed to previous square instead of 2

                    moveData.piece.GetComponent<RectTransform>().anchoredPosition =
                        moveData.piece.currentSquare.GetComponent<RectTransform>().anchoredPosition;
                    moveData.piece.SetCurrentPosition(moveData.from.gameObject.name);
                    moveData.piece.SetOldPosition(moveData.to.gameObject.name);
                    moveData.piece.currentRectTransform = moveData.piece.GetComponent<RectTransform>().anchoredPosition;
                    moveData.piece.oldRectTransform = moveData.to.GetComponent<RectTransform>().anchoredPosition;
                    
                    trackMove.RemoveAt(trackMove.Count-1);
                    
                    SetNewPieceOnThis(trackMove[^1].piece.gameObject ,trackMove[^1].to.gameObject );
            #endregion    
            
            //Calculate board state and send to engine -> good for responsiveness
        }

        public void ChangesUIBasedOnTurn()
        {
            
            foreach (Transform piece in parentTransform)
            {
                piece.gameObject.GetComponent<ChessPiece>().myTurn = false;
                switch (turn)
                {
                    case MoveTurn.WhiteToMove when !isBlack(piece.gameObject.GetComponent<ChessPiece>().pCode):
                    case MoveTurn.BlackToMove when isBlack(piece.gameObject.GetComponent<ChessPiece>().pCode):
                        piece.gameObject.GetComponent<ChessPiece>().myTurn = true;
                        break;
                }
            }
        }


        private bool isBlack(int pCode)
        {
            var code = pCode & (int)pieceCode.Black;
            return code == (int)pieceCode.Black;
        }


        #region  Mapping daa to cells
                private void MapData(int data , int index)
                {
                    // We will pass just the array  and then this should map
               
                    foreach (var item in config.pieceList)
                    {
                        var finalCode = item.code | item.color;
                        if (data.ToString() ==  finalCode.ToString())
                        { 
                            var p = Instantiate(piecePrefab,transform.position , Quaternion.identity ,parentTransform);
                           p.GetComponent<ChessPiece>().Init(item.piece_name ,item.piece_image);
                           p.GetComponent<Image>().sprite = item.piece_image;
                           p.gameObject.name = item.piece_name;
                           p.gameObject.GetComponent<ChessPiece>().pCode = (int)finalCode;
                           p.gameObject.GetComponent<ChessPiece>().pColor = (int)item.color;
                           MapDataToCell(p  ,index);
                        }
                       
                    }
                }
                //Entry
                private void MapDataToCell( GameObject p ,int index)
                {
                    //Case 1: Spawn the ui there with the specific index 
                 
                    // 64
                    for (int rank = 0; rank < 8; rank++)
                    {
                        for (int file = 0; file < 8; file++)
                        {
                            if (index != (8 * rank) + file)
                                continue;
                            ChessPiece piece = p.GetComponent<ChessPiece>();
                            //Align with all the relevant squares
                            p.transform.localPosition =  new Vector3(xOffset+ file *size, yOffset + rank*size );
                            piece.SetCurrentPosition(AssignCellNotation(file, rank));
                            piece.currentRectTransform = piece. GetComponent<RectTransform>().anchoredPosition;
                            piece.currentSquare = AssignDefaultSquare(file, rank);
                            break;
                        }
                    }

                }

                private ChessSquare AssignDefaultSquare(int file , int rank)
                {
                   
                    int index = (8 * rank) + file;
                    return chessBoardHolder.GetChild(index).GetComponent<ChessSquare>();
                }

                private string AssignCellNotation(int file, int rank)
                {
                    string square = null ; 
                    return  square= file switch
                    {
                        0 => "a" + (rank + 1),
                        1 => "b" + (rank + 1),
                        2 => "c" + (rank + 1),
                        3 => "d" + (rank + 1),
                        4 => "e" + (rank + 1),
                        5 => "f" + (rank + 1),
                        6 => "g" + (rank + 1),
                        7 => "h" + (rank + 1),
                        _ => square 
                    };
                }

                //This function will receive the list of array and perform mapping
                //IMportant for mapping the default board
                private void Map(int[] data)
                {
                    internalBoard = data;
                    for (int i = 0; i < internalBoard.Length; i++)
                    {   
                        
                        //Sending the piece data and the the square at which the piece is 
                        MapData(internalBoard[i], i);
                    }
                }

        #endregion
        
       
        public void EnforceTurnMechanic()
        {
            //
        }

        private void UpdateTurn(int move)
        { 
            int actualMove = move;
           turn =  actualMove == (int)pieceCode.White ? MoveTurn.WhiteToMove : MoveTurn.BlackToMove;
           ChangesUIBasedOnTurn();
          
        }

        #region Unity methods

        private void Start()
        {
            Event<int[]>.GameEvent +=Map;
            Event<bool>.GameEvent += ValidationResult;
            Event.MoveMade += MoveMade;
            Event.changeTurn += UpdateTurn;
        }

      

        private void Awake()
            {
                Instance = this;
            }
        
         

            private void OnDisable()
            {
                Event<int[]>.GameEvent -= Map;
                Event<bool>.GameEvent -= ValidationResult;
                Event.MoveMade -= MoveMade;
                Event.changeTurn -= UpdateTurn;

            }

          

            #endregion

    }

[Serializable]
public class PandIndex      
{
    public int index;
    public int pieceCode;
}

public enum MoveTurn
{
    //DOING BITWISE AND WITH THE PIECE CODE WE CAN FIND OUT WHO IS BLACK AND WHITE
    WhiteToMove =16 ,
    BlackToMove =32 ,
}


[Serializable]
public class MoveTracker
{
    public ChessPiece piece;
    public ChessSquare from;
    public ChessSquare to;

    public MoveTracker( ChessPiece p,ChessSquare oldCell, ChessSquare newCell)
    {
        this.piece = p;
        this.from = oldCell;
        this.to = newCell;
    }

}



