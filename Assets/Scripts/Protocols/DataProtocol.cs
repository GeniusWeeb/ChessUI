 public class DataProtocol :IPCMessage
 {
  
     public DataProtocol(string msgType ,string data)
        {
            this.msgType = msgType;
            this.data = data;

        }
    }

 public abstract class IPCMessage
 {
     public string msgType { get;  protected set; }
     public string data {get; protected set;
     }
 }




 public enum ProtocolTypes
 {
     MOVE,
     BOARDSTATE,
     GAMESTART,
     GAMEEND,
     VALIDATE
 }




 //FROM GAME STATE YOU CAN RETRIEVE LOT OF INFO
