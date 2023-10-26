 public class DataProtocol
    {

        public class Move
        {
            public string moveType { get;  set; }
            public string moveName { get;  set; }

            public Move(string name ,string moveType)
            {
                this.moveName = name;
                this.moveType = moveType;
            }
        }

    }



 public enum DataSendTypes
 {
     MOVE 
 }
 
 
 
