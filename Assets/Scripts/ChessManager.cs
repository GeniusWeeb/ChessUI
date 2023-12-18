
using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;


public class ChessManager :MonoBehaviour
    {
        #region Variables



        [SerializeField] private GameMode currentGameMode;
        [SerializeField] private MoveTurn myPieceColour; 
           
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


           public bool requestedData = false;

           [SerializeField] private GameObject capturedPiece;
           private GameObject samplefrom;
           private GameObject SampleTO;
           
       
       #endregion



       // default colour choice
       
        public static ChessManager Instance
        { get; private set;
        }
        
        private void MoveMade()
        {
            //Come From Engine as well
            moveCounter += 1;
            moveCounterTxt.text = moveCounter.ToString();
        }

        public ChessConfig GetChessConfig => config;
        
        //REQUIRES MOVE PROTOCOL
        public void SendMoveMadeToEngine(int currentSquare , int targetSquare)
        {
            
          
            //We dont have the piece name here
            bool canMakeMove = false;
            string move = currentSquare + "-" + targetSquare;
            var colorCode = (int)turn;
            DataProtocol moveData = new DataProtocol(ProtocolTypes.MOVE.ToString() , move , colorCode.ToString()  );
            Connection.Instance.SendMessage(moveData);
            Debug.Log( $"<color=red> {move} </color>");
        }
   
        private void ValidationResult( bool canMove)
        {

            if (canMove)
            {   
              //  //Debug.log("Move can take place");
                PerformMoveFinal();
            }
            else
            {
                ////Debug.log("Move cant take place");
                ResetPiecePosition();
            }

          
        }
        
        private void ResetPiecePosition()
        {   
            pieceThatMadeMove.GetComponent<RectTransform>().anchoredPosition =
                pieceThatMadeMove.GetComponent<ChessPiece>().currentRectTransform;
            if(capturedPiece!=null)    capturedPiece.SetActive(true);
            SetCapturePiece(null);

            if (pieceThatMadeMove == null) return;
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

            if (pieceThatMadeMove == null || squareThatPieceMovedTo == null)
            {
                Debug.Log("<color=yellow> Already returning</color>");
                return;
            }
            SetCapturePiece(null);
            GameObject p = pieceThatMadeMove;
            GameObject newSquare = squareThatPieceMovedTo;
            // IF MOVE IS NOT CORRECT , SNAP IT BACK TO OLD POSITION AND JUST RETURN
            //CONFIRMATION THAT THE MOVE IS SUCCESSFUL
            
            ChessPiece currentP = p.GetComponent<ChessPiece>();
            
            //Setting previous square
            currentP.previousSquare = currentP.currentSquare;
            currentP.previousSquare.currentP = null;
            
            //updating the new square
            squareThatPieceMovedTo.GetComponent<ChessSquare>().currentP = currentP;
            currentP.currentSquare = squareThatPieceMovedTo.GetComponent<ChessSquare>();
            
            
            
            currentP.oldRectTransform = currentP.currentRectTransform;
            currentP.currentRectTransform = newSquare.GetComponent<RectTransform>().anchoredPosition;
            currentP.SetOldPosition(currentP.GetCurrentPosition);
            currentP.SetCurrentPosition(newSquare.gameObject.name);
            currentP.GetComponent<RectTransform>().anchoredPosition = newSquare.GetComponent<RectTransform>().anchoredPosition;

            
          
            MoveMade();
         

        }

        public bool isConsoleReady = false;
        

        public void RequestPossibleCellDataForThisIndex(int index)
        {   //send index here

            string  data = index.ToString();
           DataProtocol moveData = new DataProtocol(ProtocolTypes.INDICATE.ToString(), data, null);
           Connection.Instance.SendMessage(moveData);
        }


        //ui interaction
     

        public void UndoMain()
        {
            DataProtocol undoMove = new DataProtocol(ProtocolTypes.UNDO.ToString(), null, null);
            Connection.Instance.SendMessage(undoMove);

        }

        //ALLOW MOVEMENT

        private void ChangesUIBasedOnTurn()
        {
            
            foreach (Transform piece in parentTransform)
            {   
                piece.gameObject.GetComponent<ChessPiece>().myTurn = false;
                if (turn != myPieceColour && currentGameMode == GameMode.PlayerVsBot) return;
                switch (turn)
                {
                    case MoveTurn.WhiteToMove when !isBlack(piece.gameObject.GetComponent<ChessPiece>().pCode)  :
                    case MoveTurn.BlackToMove when isBlack(piece.gameObject.GetComponent<ChessPiece>().pCode)  :
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

        public GameObject GetCapturedPiece => capturedPiece;

        public void SetCapturePiece(GameObject p)
        {
            capturedPiece = p;
        }

        
        private void UpdateUIFromEngine(List<int> FromToIndexData)
        {
            int fromIndex = FromToIndexData[0];
            int toIndex = FromToIndexData[1];
            
            Debug.LogError("Count of data is => " + FromToIndexData.Count);
            
            GameObject fromObject =null, toObject =null;

            foreach (Transform item in chessBoardHolder)
            {
                var p = item.gameObject.GetComponent<ChessSquare>();
                if (p.currentIndex == fromIndex)
                {
                    fromObject = p.gameObject;
                   
                }
                if (p.currentIndex == toIndex)
                {
                    toObject = p.gameObject;
                 
                    
                }
            }
            var from = fromObject.gameObject.GetComponent<ChessSquare>();
            if (from.currentP == null) {
                Debug.Log($"<color=yellow> From  index {fromIndex} P is null</color>");
                return;
            }
            CheckIfPieceCapturedUI(toObject);
            SetNewPieceOnThis(from.currentP.gameObject, toObject);
            PerformMoveFinal();
            if (FromToIndexData.Count == 3)
                PerformVisualForCapturedPiece(FromToIndexData[2],fromObject);
            
        }

        private void PerformVisualForCapturedPiece(int pCode , GameObject CapturedPiecePos )
        {
            //if not en passant , then 
            foreach (Transform piece in parentTransform )
            {
                if(piece.gameObject.activeInHierarchy)
                    continue;
                ChessPiece capPieceToShow = piece.GetComponent<ChessPiece>();
                ChessSquare oldSquareForCapPiece = CapturedPiecePos.GetComponent<ChessSquare>();
                if (capPieceToShow.pCode == pCode && capPieceToShow.currentSquare == oldSquareForCapPiece)
                {
                    oldSquareForCapPiece.currentP = capPieceToShow;
                    piece.gameObject.SetActive(true);
                   

                }

            }
            
        }

        private void CheckIfPieceCapturedUI(GameObject toSquare)
        {
            ChessSquare sq = toSquare.GetComponent<ChessSquare>();

            if (sq.currentP != null)
            {
                Debug.Log("Capturing piece");
                sq.currentP.gameObject.SetActive(false);
                SetCapturePiece(sq.currentP.gameObject);
            }
        }

        public void SetUpGame()
        {
            var dataToSend = currentGameMode + "-" + (int)myPieceColour;
            DataProtocol finalData = new DataProtocol(ProtocolTypes.GAMEMODE.ToString(),dataToSend,null);
            if (!isConsoleReady && currentGameMode == GameMode.None ) return;
            Connection.Instance.SendMessage(finalData);

        }

        public MoveTurn GetMyPColor => myPieceColour; 

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
                            piece.currentSquare.GetComponent<ChessSquare>().currentP = piece;
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


        private void ConnectedToConsole()
        {
            isConsoleReady = true;
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
            Event<List<int>>.GameEvent += UpdateUIFromEngine;
            Event.ConnectedToConsole += ConnectedToConsole;
           

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
                Event<List<int>>.GameEvent -= UpdateUIFromEngine;
                Event.ConnectedToConsole += ConnectedToConsole;


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

public enum GameMode
{
    PlayerVsPlayer ,
    PlayerVsBot ,
    BotVsBot,
    None
}




