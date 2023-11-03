
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
        public void SendMoveMadeToEngine(string pieceName , string squareName)
        {
            Debug.Log("HE TRYNA MAKE A MOVE");
            bool canMakeMove = false;
            string move = pieceName + "-" + squareName;
            //moveHistory.Add(move);
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
            if(capturedPiece!=null)     capturedPiece.SetActive(true);
           
            
            
            SetCapturePiece(null);
            pieceThatMadeMove = null;
            squareThatPieceMovedTo = null;
        }

        //Just set temporary
        public void SetNewPieceOnThis(GameObject p , GameObject newSquare)
        {
            pieceThatMadeMove = p;
            squareThatPieceMovedTo = newSquare;
            Debug.Log($"piece is {p.name} and square is {newSquare.name}");
        }
        private void PerformMoveFinal()
        {
            if (pieceThatMadeMove == null || squareThatPieceMovedTo == null)
                return;
            SetCapturePiece(null);
            GameObject p = pieceThatMadeMove;
            GameObject newSquare = squareThatPieceMovedTo;
            // IF MOVE IS NOT CORRECT , SNAP IT BACK TO OLD POSITION AND JUST RETURN
            //CONFIRMATION THAT THE MOVE IS SUCCESSFUL
           // ChessPiece  currentP = newSquare.GetComponent<ChessSquare>().currentP;
            ChessPiece currentP = p.GetComponent<ChessPiece>();
            //UPDATE TRANSFORMS ONCE MOVE HAS BEE MADE
            currentP.oldRectTransform = currentP.currentRectTransform;
            currentP.currentRectTransform = newSquare.GetComponent<RectTransform>().anchoredPosition;
            currentP.previousSquare = currentP.currentSquare;
            currentP.currentSquare = newSquare.gameObject.GetComponent<ChessSquare>();
            currentP.SetOldPosition(currentP.GetCurrentPosition);
            currentP.SetCurrentPosition(newSquare.gameObject.name);
            currentP.GetComponent<RectTransform>().anchoredPosition =
                newSquare.GetComponent<RectTransform>().anchoredPosition;

            
            Debug.Log($"current p is {currentP}  and previous squre {currentP.previousSquare} nad curren square is {currentP.currentSquare}");
           
            trackMove.Add( new MoveTracker( currentP,currentP.previousSquare ,currentP.currentSquare));
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

            StartCoroutine((delay(fromObject.GetComponent<ChessSquare>().currentP.gameObject, toObject)));

        }

        public void SetUpGame()
        {
            var dataToSend = currentGameMode + "-" + (int)myPieceColour;
            Debug.Log($"mode is {currentGameMode} and  colour chosen is {myPieceColour}");
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
    

        public IEnumerator delay(GameObject fromObject , GameObject toObject)
        {
            yield return new WaitForSeconds(1f);
            SetNewPieceOnThis(fromObject, toObject);
            PerformMoveFinal();
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




