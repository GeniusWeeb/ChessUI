
    using System;

    public static class Event<T>
    {
        public static  Action<T> GameEvent = delegate(T obj) {  };
        public static  Action<T> GameUIShowCell = delegate(T obj) {  };
     
    }

    public static class Event
    {
        public static  Action MoveMade;
        public static Action<int> changeTurn = delegate(int i) {  };
        public static Action ResetCellColor;
    }

 