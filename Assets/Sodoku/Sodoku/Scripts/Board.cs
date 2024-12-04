using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Soduku
{
    public class Board : MonoBehaviour
    {
        [Header("Board")]
      //  public Sprite[] _spriteBoard;
        public GridLayoutGroup gridLayout;
        public Vector2 vecCellSize;
        public Vector2 vecSpacing;

        [Header("Number")]
        [SerializeField ]private GameObject numbers;
        [SerializeField] private HorizontalLayoutGroup horizontalLayout;
        [SerializeField] private Transform parentNumbers;
        public void CreateBoard(int boardSize)
        {
            switch (boardSize)
            {
                case 6:
                  //  spriteBoard.sprite = _spriteBoard[0];
                    vecCellSize = new Vector2(450, 300);
                    vecSpacing = new Vector2(41, 20);
                    SpawnNumber(boardSize);
                    break;
                case 9:
                   // spriteBoard.sprite = _spriteBoard[1];
                    SpawnNumber(boardSize);
                    vecCellSize = new Vector2(308, 308);
                   vecSpacing = new Vector2(15, 15);
                    break;               
                default:
                    Debug.LogError("Loi roi con dau");
                    break;
            }
           // gridLayout.constraintCount = boardSize;
            gridLayout.cellSize = vecCellSize;
            gridLayout.spacing = vecSpacing;
        }
        private Dictionary<GameObject, int> dicNumbers;
        void SpawnNumber(int size)
        {
            dicNumbers = new Dictionary<GameObject, int>();
            for(int i = 0; i < size; i++)
            {
                GameObject number = Instantiate(numbers, parentNumbers);
                number.GetComponent<TextMeshProUGUI>().text=(i+1).ToString();
                dicNumbers[number] = i + 1;
                number.GetComponent<Button>().onClick.AddListener(() => SelectValue(dicNumbers[number]));
            }
            horizontalLayout.spacing = 14f;
        }
        void SelectValue(int value)
        {
            Debug.Log("Addlisten " + value);
            GamePlay.instance.UpdateValueCell(value);

        }
    }
}
