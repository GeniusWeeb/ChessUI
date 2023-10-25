
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChessManager :MonoBehaviour
    {

        [SerializeField] private ChessConfig config;
        [SerializeField] private Transform parentTransform;
        [SerializeField] private GameObject piecePrefab;
        [SerializeField] private int  xOffset;
        [SerializeField] private int  yOffset;
        [SerializeField] private int  size;


       [SerializeField] private List<PandIndex> indexs;
        
        private ChessPiece cp;
        
        public static ChessManager Instance
        { get; private set;
        }
        
        private void MapData(string data , int index)
        {
            // We will pass just the array  and then this should map
            Debug.Log("Now checking for pieces");
            foreach (var item in config.pieceList)
            {
                var finalCode = item.code | item.color;
                if (data.ToString() ==  finalCode.ToString())
                {
                    Debug.Log("Found it");
                    var p = Instantiate(piecePrefab,transform.position , Quaternion.identity ,parentTransform);
                   p.GetComponent<ChessPiece>().Init(item.piece_name ,item.piece_image);
                   p.GetComponent<Image>().sprite = item.piece_image;
                   MapDataToCell(p  ,index);
                }
               
            }
        }
        //Entry
        private void MapDataToCell( GameObject p ,int index)
        {
            
            //Case 1: Spawn the ui there with the specific index 
            Debug.Log("Mapping to cells");
            // 64
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    Debug.Log( (8 * rank + file));
                    if (index != (8 * rank) + file)
                        continue;
                    Debug.Log("Adding loacl postion");
                    p.transform.localPosition =  new Vector3(xOffset+ file *size, yOffset + rank*size ); 
                    break;
                }
            }

        }
    
        
        //This function will receive the list of array and perform mapping
        //IMportant for mapping the default board
        private void Map(string data)
        {
            
            foreach (var item in indexs )
            {
                MapData(item.pieceCode.ToString(), item.index);
            }
        }



        #region Unity methods
        
            private void Awake()
            {
                Instance = this;
               
            }

            private void OnEnable()
            {
                Event.IncomingData +=Map;

            }

            void test()
            {
                Debug.Log("Succesffuly invoked");
            }


            private void OnDisable()
            {
                Event.IncomingData -= Map;
            }

        #endregion

    }

[Serializable]
public class PandIndex
{
    public int index;
    public int pieceCode;
}
