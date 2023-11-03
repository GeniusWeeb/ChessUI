

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;


public class Connection : MonoBehaviour
{


    public static Connection Instance { get; private set; }


    private WebSocket ws;
    public WebSocket GetWebSocket => ws;
    
    [SerializeField]private string zimServer = "ws://127.0.0.1:8281";


    private void Awake()
    {
        Instance = this;
    }
    
    //[ContextMenu("Start Connection")]
   public void Start()
   {
       Debug.Log("Trying to connect");
       StartCoroutine(ConnectToServer());
   }

   IEnumerator ConnectToServer()
   {    
       
       Uri url = new Uri(zimServer);
       ws = new WebSocket(url.AbsoluteUri);
       ws.OnOpen += ServerConnected;
       ws.OnMessage += (sender, e) => HandleWebSocketMessage(e.Data);
      // ws.OnError += (sender, e) => Debug.LogError("WebSocket error: " + e.Message);
       ws.OnClose += (sender, e) => Debug.Log($"WebSocket closed. Code: {e.Code}, Reason: {e.Reason}");

       ws.Connect();
     
       while (ws.IsAlive)
       {
           yield return null;
       }
   }

   private void ServerConnected(object sender, EventArgs e) 
   {
       Debug.Log("Connected to console");
   }
   
   private void  HandleWebSocketMessage(string data)
   { 
       Debug.Log("<color=red> Received Data </color>" + data);
       DataProtocol incomingData = JsonConvert.DeserializeObject<DataProtocol>(data);
      
       Debug.Log(incomingData.msgType);
       
       //DEFAULT GAME START
      if (incomingData.msgType == ProtocolTypes.GAMESTART.ToString())
      { 
        
          var board = JsonConvert.DeserializeObject<int[]>(incomingData.data);
      
          MainThreadDispatcher.EnQueue(
              () =>
              { 
                  Event<int[]>.GameEvent.Invoke(board);
                  Event.changeTurn (JsonConvert.DeserializeObject<int>(incomingData.toMove));
              });
          
      }
      //ALLOWS MOVEMENT BASED ON VALIDATION FROM THE ENGINE
      else if (incomingData.msgType == ProtocolTypes.VALIDATE.ToString())
      { 
          bool canMakeMove = JsonConvert.DeserializeObject<bool>(incomingData.data);
          MainThreadDispatcher.EnQueue(
              () =>
              {
                    Event<bool>.GameEvent.Invoke(canMakeMove);
                    Event.changeTurn (JsonConvert.DeserializeObject<int>(incomingData.toMove));
              });
          
      }
      
      //RECEIVES DATA THAT WILL SHOW THE SQUARES WE CAN MOVE TO
      else if (incomingData.msgType == ProtocolTypes.INDICATE.ToString())
      {
       
          HashSet<int> squares = JsonConvert.DeserializeObject<HashSet<int>>(incomingData.data);
         
          MainThreadDispatcher.EnQueue(
              () =>
              {
                  Event<HashSet<int>>.GameUIShowCell.Invoke(squares);
                  
              });
      }
      else if (incomingData.msgType == ProtocolTypes.UPDATEUI.ToString())
      {
       
          List<int> newIndexUpdate = JsonConvert.DeserializeObject<List<int>>(incomingData.data);
          
          MainThreadDispatcher.EnQueue(
              () =>
              {
                  Event<List<int>>.GameEvent.Invoke(newIndexUpdate);
                  
              });
      }
   }


   public void SendMessage<T>(T data )
   {
       var toSend =JsonConvert.SerializeObject(data);
       Debug.Log(toSend);
       ws.Send(toSend);
   }

   private void OnDestroy()
   {
       ws.Close();
   }
   
   
}