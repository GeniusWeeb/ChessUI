using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CreateAssetMenu(menuName = "Chess/chessConfig")]
public class ChessConfig : ScriptableObject
{

    public List<Piece> pieceList;
    
}

[System.Serializable]
public class Piece
{
    public string piece_name;
    public Sprite piece_image;
    public pieceCode code;
    public pieceCode color;
    
}


public enum pieceCode
{   
   empty =  0 , 
   Pawn =   1 , 
   Rook =   2 ,
   Knight = 3 , 
   Bishop = 4 ,
   Queen =  6 ,
   King  =  8 ,
   
   
   //Black or white
   White = 16 ,
   Black = 32
}




