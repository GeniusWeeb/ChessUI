using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class MainThreadDispatcher : MonoBehaviour
    {

        private static readonly Queue<Action> executionQueue = new Queue<Action>();

        private void Update()
        {
            lock (executionQueue)
            {
                while (executionQueue.Count > 0 )
                {
                    executionQueue.Dequeue().Invoke();
                    
                }
                
            }
        }


        public static void EnQueue(Action action)
        {
            lock (executionQueue)
            {
                executionQueue.Enqueue(action);
            }
        }
    }
