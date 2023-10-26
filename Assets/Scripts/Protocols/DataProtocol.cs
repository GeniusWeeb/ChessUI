 public class DataProtocol :IPCMessage
 {
     public string msgType { get; set; }
     public string data { get; set;
     }

     public DataProtocol(string msgType ,string data)
        {
            this.msgType = msgType;
            this.data = data;

        }
    }

 public abstract class IPCMessage
 {
     public string msgType { get; set; }
     public string data {get; set;
     }
 }




 public enum ProtocolTypes
 {
     MOVE, BOARDSTATE , GAMESTART , GAMEEND
 }
 
 
 
 
 //FROM GAME STATE YOU CAN RETRIEVE LOT OF INFO
