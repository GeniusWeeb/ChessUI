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
    [SerializeField] private ChessConfig  config;
    


    [SerializeField] private string myMessage;
    

    private void Start()
    {
        CreateBoardUI();
    }
    private void CreateBoardUI()
    {
        for (int rank = 0; rank < 8; rank++)
        {
            for (int file = 0; file < 8; file++)
            {
                var obj = Instantiate(tile ,transform.localPosition,Quaternion.identity);
                obj.transform.SetParent(panel);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition=  new Vector3(xOffset+ file *size, yOffset + rank*size );
                obj.GetComponent<Image>().color = (file + rank) % 2 == 0 ? Color.grey : Color.white;

                obj.name = file switch
                {
                    0 => "a" + (rank + 1),
                    1 => "b" + (rank + 1),
                    2 => "c" + (rank + 1),
                    3 => "d" + (rank + 1),
                    4 => "e" + (rank + 1),
                    5 => "f" + (rank + 1),
                    6 => "g" + (rank + 1),
                    7 => "h" + (rank + 1),
                    _ => obj.name
                };
            }
            
        }
    }
    

    [ContextMenu("Send a message to my bitch console")]
    public void SendMessage()
    {
        Connection.Instance.GetWebSocket.Send(myMessage);
    }

}