using System;
using System.Threading;
using TreeEditor;
using UnityEngine;
using UnityEngine.UI;


public class ChessManager :MonoBehaviour
    {

        [SerializeField] private ChessConfig config;
        [SerializeField] private Transform parentTransform;
        [SerializeField] private GameObject piecePrefab;
        private ChessPiece cp;
        
        
        
    
        public static ChessManager Instance
        { get; private set;
        }
        

        private void MapData(string data)
        {   
            
            Debug.Log("NOw checking for pieces");
            foreach (var item in config.pieceList)
            {   
           
                var finalCode = item.code | item.color;
               if (data.ToString() ==  finalCode.ToString())
                {
                    Debug.Log("Found it");
                    var p = Instantiate(piecePrefab,transform.position , Quaternion.identity ,parentTransform);
                   p.GetComponent<ChessPiece>().Init(item.piece_name ,item.piece_image);
                   p.GetComponent<Image>().sprite = item.piece_image;
                  p.transform.localPosition = Vector3.zero;
                   Debug.Log(p.name);
                   Debug.Log(item.color + "and " + item.code);
                }
               
               
            }
        }

     

        #region Unity methods
        
            private void Awake()
            {
                Instance = this;
               
            }

            private void OnEnable()
            {
                Event.IncomingData += MapData;

            }

            void test()
            {
                Debug.Log("Succesffuly invoked");
            }


            private void OnDisable()
            {
                Event.IncomingData -= MapData;
            }

        #endregion

    }
