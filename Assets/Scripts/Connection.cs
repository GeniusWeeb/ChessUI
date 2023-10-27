

using System;
using System.Collections;
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
      if (incomingData.msgType == ProtocolTypes.GAMESTART.ToString()) {
          var board = JsonConvert.DeserializeObject<int[]>(incomingData.data);
          MainThreadDispatcher.EnQueue(
              () =>
              { 
                  Event<int[]>.GameEvent.Invoke(board);
              });
          
      }
      
      else if (incomingData.msgType == ProtocolTypes.VALIDATE.ToString())
      { 
          bool canMakeMove = JsonConvert.DeserializeObject<bool>(incomingData.data);
          MainThreadDispatcher.EnQueue(
              () =>
              {
                    Event<bool>.GameEvent.Invoke(canMakeMove);
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