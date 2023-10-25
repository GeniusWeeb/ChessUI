using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
   
    
    [SerializeField] private Material[] mats;
    [SerializeField] private GameObject tile;
    [SerializeField] private int file;
    [SerializeField] private int rank;
    [SerializeField] private Transform panel;
    [SerializeField] private int size;
    [SerializeField] private int xOffset;
    [SerializeField] private int yOffset;



    [SerializeField] private string myMessage;
    

    private void Start()
    {
        CreateBoardUI();
    }
    private void CreateBoardUI()
    {
        for (int i = 0; i < file; i++)
        {
            for (int j = 0; j < rank; j++)
            {
                var obj = Instantiate(tile ,transform.localPosition,Quaternion.identity);
                obj.transform.SetParent(panel);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition=  new Vector3(xOffset+ j *size, yOffset + i*size );
                obj.GetComponent<Image>().color = (i + j) % 2 == 0 ? Color.black : Color.white;
                
            }
            
        }
    }
    
    [ContextMenu("Basic Piece setup")]
    public void SetupDefaultPieces()
    {
        //PPPPPPPP
        //RNBQKBNR
        
        
        // This default will come from the Engine
        //We should have marked way to identify which is what here
        //Later based on FEN we should update the board state 
        
    }


    [ContextMenu("Send a message to my bitch console")]
    public void SendMessage()
    {
        Connection.Instance.GetWebSocket.Send(myMessage);
    }

}