
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;


public class ChessManager :MonoBehaviour
    {
    
        [SerializeField] private List<string> moveHistory;
        
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
            DataProtocol moveData = new DataProtocol(ProtocolTypes.MOVE.ToString() , move );
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
                            break;
                        }
                    }

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

        #region Unity methods

        private void Start()
        {
            Event<int[]>.GameEvent +=Map;
            Event<bool>.GameEvent += ValidationResult;
            Event.MoveMade += MoveMade;
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
    WhiteToMove, 
    BlackToMove, 
}



