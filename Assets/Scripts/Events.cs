
    using System;

    public static class Event<T>
    {
        public static  Action<T> GameEvent = delegate(T obj) {  };
    }

    public static class Event
    {
        public static  Action MoveMade; 
    }

 