using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Soduku
{
    public class GamePlay : MonoBehaviour
    {
        public static GamePlay instance;
        [Header("Board")]
        public Image spriteBoard;
        public Board boardSodoku;
        private Cell[,] cells;
        [SerializeField] private int grid_Col = 3;
        [SerializeField] private int grid_Row = 2;
        [SerializeField] private int typeRow = 2;
        [SerializeField] private ListCell cell3x2;
        [SerializeField] private ListCell cell3x3;
        private ListCell listCell;
        public GameObject gridParent;
        private int[,] levelAns;
        private int[,] array;
        public UI_Manager UI;

        public Sprite[] sprBackGround;
        public Sprite[] sprSelect;
        public Sprite[] sprWrong;
        public Sprite[] sprCheck;
        public bool isPlaying = true;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            // CameraFit();
        }
        private void ChangeSprite(int ran,Cell cell)
        {
            if (ran != 0)
            {
                cell.sprBG.gameObject.SetActive(false);
                cell.sprBG.sprite = sprBackGround[ran];
                cell.selectImage = sprSelect[ran];
                cell.wrongImage = sprWrong[ran];
                cell.checkImage = sprCheck[ran];
            }
            else
            {
                cell.sprBG.gameObject.SetActive(false);
                cell.selectImage = sprSelect[ran];
                cell.wrongImage = sprWrong[ran];
                cell.checkImage = sprCheck[ran];
            }
        }
        #region Effect 1 Row Or 1 Col Or Box
        private bool[,] checkHC = new bool[10, 10];
        private void FLood(int row,int col,int type,int posX=0,int posY=9,int dis=0)
        {
            if(type == 0)
            {
               // Debug.Log("================Row===========");
                for (int i = 0; i <= col; i++)
                {
                    StartCoroutine(DelayDisplayEffect(row, i));
                }
                for(int i = col + 1; i < grid_Row * grid_Col; i++)
                {
                    StartCoroutine(DelayDisplayEffect(row, i));
                }

            }
            else if(type==1)
            {
               // Debug.Log("================Col===========");
                for (int i = 0; i <= row; i++)
                {
                    StartCoroutine(DelayDisplayEffect(i, col));
                }
                for (int i = row + 1; i < grid_Row * grid_Col; i++)
                {
                    StartCoroutine(DelayDisplayEffect(i, col));
                }
            }
            else
            {
                checkHC[row, col] = true;
                if (row < posX + type - 1 && checkHC[row + 1, col] == false)
                {
                    FLood(row + 1, col, type, posX, posY, dis);
                }
                if (col > posY && checkHC[row, col - 1] == false)
                {
                    FLood(row, col - 1, type, posX, posY, dis);
                }
                if (col < posY + dis - 1 && checkHC[row, col + 1] == false)
                {
                    FLood(row, col + 1, type, posX, posY, dis);
                }
                if (row > posX && checkHC[row - 1, col] == false)
                {
                    FLood(row - 1, col, type, posX, posY, dis);
                }
                StartCoroutine(DelayDisplayEffect(row, col));
            }
        }
        IEnumerator DelayDisplayEffect(int row,int col) {

            cells[row,col].effect.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f);         
            cells[row, col].effect.color = new Color(1, 1, 1, 1);
            cells[row, col].effect.DOFade(0f, 1f);
            yield return new WaitForSeconds(0.5f / (typeRow*3));
        }
        private void OnDisable()
        {
            spriteBoard.DOKill();
         
        }
        public void Effec1RowOrCol(int row,int col)
        {
            UI.Destroy1Number(cells[row, col].value);
            checkHC = new bool[10, 10];
            int startRow = (row / typeRow) * typeRow;
            int startCol = (col / 3) * 3;
            bool checkRow = true;
            for(int i=0;i<grid_Col*grid_Row;i++)
            {
                if (cells[row, i].isIncorrect == true)
                {
                    checkRow = false;
                    break;
                }
            }
            bool checkCol = true;
            for (int i = 0; i < grid_Col * grid_Row; i++)
            {
                if (cells[i, col].isIncorrect == true)
                {
                    checkCol = false;
                    break;
                }
            }
            bool checkBox = true;
            for(int i = 0; i < grid_Row * grid_Col; i++)
            {
                if(cells[startRow + i / 3, startCol + i % 3].isIncorrect== true)
                {
                    checkBox= false; break;
                }
            }
            if(checkRow==true)
            {
                PlaySound_Box();
                FLood(row, col,0);
            }
            else if(checkCol == true)
            {
                PlaySound_Box();
                FLood(row, col, 1);
            }
            else if (checkBox == true)
            {
                PlaySound_Box();
                FLood(row, col, typeRow,startRow,startCol,3);
            }
        }
        #endregion
        public void ChooseDifficulty(int clickType)
        {
          //  Debug.Log("====xxxxxDelay------");
            spriteBoard.fillAmount =1;
            //spriteBoard.DOFillAmount(0f, 1.2f).SetEase(Ease.OutSine);
            if (clickType == 0)
            {
                boardSodoku.CreateBoard(6);
                grid_Col = 3; grid_Row = 2; typeRow = 2;
                listCell = cell3x2;
            }
            else if (clickType == 1 || clickType == 2|| clickType == 3)
            {
                boardSodoku.CreateBoard(9);
                grid_Col = 3; typeRow = 3; grid_Row = 3;
                listCell = cell3x3;
            }
            cells = new Cell[grid_Row * grid_Col, grid_Col * grid_Row];
            levelAns = new int[grid_Row * grid_Col, grid_Row * grid_Col];
            array = new int[grid_Row * grid_Col, grid_Row * grid_Col];
            SpawnCell(clickType);
            savePreviousStep.Clear();
            spriteBoard.DOFillAmount(0f, 1.2f).SetEase(Ease.OutSine);
        }
        private bool typeMap = false;
        public void ResetMap()
        {
            savePreviousStep.Clear();
            if (typeMap == true)
            {
                for(int i=0;i< grid_Row * grid_Col; i++)
                {
                    for(int j=0;j< grid_Row * grid_Col; j++)
                    {
                        if (array[i,j] == 0)
                        {
                            cells[i, j].Init(levelAns[i,j]);
                            UI.Destroy1Number(cells[i,j].value);
                        }
                        else
                        {
                            cells[i, j].Init(0);
                        }
                        cells[i, j].Reset();
                    }
                }
            }
            else
            {
                for (int i = 0; i < grid_Row * grid_Col; i++)
                {
                    for (int j = 0; j < grid_Row * grid_Col; j++)
                    {
                        if (array[i, j] != 0)
                        {
                            cells[i, j].Init(levelAns[i, j]);
                            UI.Destroy1Number(cells[i, j].value);
                        }
                        else
                        {
                            cells[i, j].Init(0);
                        }
                        cells[i, j].Reset();
                    }
                }
            }         
        }
        private void SpawnCell(int clickType)
        {
          //  Debug.Log("====yyyyyDelay------");
            Generator gen = new Generator();
            string s = "";
            Array.Copy(gen.CreateLevel(grid_Row * grid_Col, typeRow), levelAns, levelAns.Length);
            Array.Copy(gen.MaskBox(grid_Row * grid_Col, typeRow, clickType), array, array.Length);
            int ran = UnityEngine.Random.Range(0, sprBackGround.Length);
            int rowH = UnityEngine.Random.Range(0, grid_Row * grid_Col);
            int colH= UnityEngine.Random.Range(0, grid_Row * grid_Col);
            for (int i = 0; i < grid_Row * grid_Col; i++)
            {
                ListCell feild = Instantiate(listCell, Vector3.zero, Quaternion.identity, gridParent.transform);
              //  feild.transform.DOScale(1f, 0.2f).SetEase(Ease.InBack).SetDelay(i/3-0.2f);
                List<Cell> arrayCell = feild.cells;
                int startRow = (i / typeRow) * typeRow;
                int startCol = (i % typeRow) * 3;
                if (clickType < 3)
                {
                    typeMap = false;
                    for (int j = 0; j < grid_Row * grid_Col; j++)
                    {
                        arrayCell[j].row = startRow + j / 3;
                        arrayCell[j].col = startCol + j % 3;
                        arrayCell[j].Init(array[arrayCell[j].row, arrayCell[j].col]);
                        cells[arrayCell[j].row, arrayCell[j].col] = arrayCell[j];
                        ChangeSprite(ran, arrayCell[j]);
                        UI.Destroy1Number(arrayCell[j].value);                     
                    }
                }
                else
                {
                    typeMap = true;
                    for (int j = 0; j < grid_Row * grid_Col; j++)
                    {
                        arrayCell[j].row = startRow + j / 3;
                        arrayCell[j].col = startCol + j % 3;
                        if (array[arrayCell[j].row, arrayCell[j].col]==0) {
                            arrayCell[j].Init(levelAns[arrayCell[j].row, arrayCell[j].col]);
                            cells[arrayCell[j].row, arrayCell[j].col] = arrayCell[j];
                            ChangeSprite(ran, arrayCell[j]);
                            UI.Destroy1Number(arrayCell[j].value);
                            
                        }
                        else
                        {
                            arrayCell[j].Init(0);
                            cells[arrayCell[j].row, arrayCell[j].col] = arrayCell[j];
                            ChangeSprite(ran, arrayCell[j]);                          
                           // UI.Destroy1Number(arrayCell[j].value);
                        }                     
                    }
                }
                // Debug.Log(startRow+" : "+ startCol +" i: "+i );                           
            }
            //Khoi dong man choi
            int startRowH = (rowH / typeRow) * typeRow;
            int startColH = (colH / 3) * 3;
            for (int i = 0; i < grid_Row * grid_Col; i++)
            {
                cells[i, colH].Reset();
                cells[i, colH].HightLight();
                cells[rowH, i].Reset();
                cells[rowH, i].HightLight();
                cells[startRowH + i / 3, startColH + i % 3].Reset();
                cells[startRowH + i / 3, startColH + i % 3].HightLight();
            }
            cells[rowH, colH].Changecolor();
        }

        #region Tao hieu ung khi chon dap an
        private Cell selectCell;
        private List<PreviousStep> savePreviousStep = new List<PreviousStep>();
        public void SelectCell(int row, int col)
        {
            if (isPlaying == false) return;
            ResetGird();
            PlaySound_ClickBox();
            int startRow = (row / typeRow) * typeRow;
            int startCol = (col / 3) * 3;
            for (int i = 0; i < grid_Row * grid_Col; i++)
            {
                if (cells[row, col].value != 0)
                {
                    if ((i != row) && (cells[i, col].value == cells[row, col].value))
                    {
                        cells[i, col].WrongValue();
                    }
                    else if (i != row)
                    {
                        cells[i, col].HightLight();
                    }
                    if ((i != col) && (cells[row, i].value == cells[row, col].value)) { cells[row, i].WrongValue(); }
                    else if (i != col) { cells[row, i].HightLight(); };
                    if (((startRow + i / 3) != row || (startCol + i % 3) != col) && (cells[startRow + i / 3, startCol + i % 3].value == cells[row, col].value))
                    {
                        cells[startRow + i / 3, startCol + i % 3].WrongValue();
                    }
                    else if ((startRow + i / 3) != row || (startCol + i % 3) != col)
                    {
                        cells[startRow + i / 3, startCol + i % 3].HightLight();
                    }
                }
                else {
                    cells[i, col].HightLight();
                    cells[row, i].HightLight();
                    cells[startRow + i / 3, startCol + i % 3].HightLight();
                }              

            }
            if (cells[row,col].value!=0&&cells[row, col].isIncorrect)
            {
                cells[row, col].WrongValue();
            }
            else
            {
                cells[row, col].Changecolor();
            }
            
            selectCell = cells[row, col];
            for (int i = 0; i < grid_Col * grid_Row; i++)
            {
                for (int j = 0; j < grid_Row * grid_Col; j++)
                {
                    cells[i, j].ResetNumber();
                }
            }
            if (useNote == true && selectCell.value!=0)
            {
              //  Debug.Log(useNote + "Node");
                for (int i = 0; i < grid_Col * grid_Row; i++)
                {
                    for (int j = 0; j < grid_Row * grid_Col; j++)
                    {
                        if (selectCell.value!=0 && cells[i, j].suggest[selectCell.value - 1] == -1)
                        {
                         //   Debug.Log($"row {i} col {j} val {selectCell.value}");
                            cells[i,j].DisplayNumberInNote(selectCell.value - 1);
                        }
                    }
                }
            }
            
        }
        public void UpdateValueCell(int value)
        {
            Debug.Log(value);
            if (isPlaying == false) return;
            if (selectCell == null) return;
            if (selectCell.isIncorrect == false) return;
            ResetGird();
            selectCell.UpdateValue(value);
            CheckValueSame();
            Effec1RowOrCol(selectCell.row,selectCell.col);
            useSuggest = false;
            if (CheckWin())
            {
                Invoke("AppearUIWin", 1f);
            }
            //Debug.Log($"row: {selectCell.row} + col: {selectCell.col} + value: {selectCell.value}");
            PreviousStep previous = new PreviousStep();
            previous.row = selectCell.row; previous.col = selectCell.col;
            previous.value = value;
            savePreviousStep.Add(previous);
            if (selectCell.isIncorrect == false && useNote == true)
            {
                useNote = false;
                CancelEhance();
            }
            if (selectCell.isIncorrect == true)
            {
                UI.CurrentHealth -= 1;
            }
            //foreach( PreviousStep step in savePreviousStep)
            //{
            //    Debug.Log($"cnt: {savePreviousStep.Count} + row: {step.row} + col: {step.col} + value: {step.value}");
            //}
        }
     
        void ResetGird()
        {
            string s = "";
            for (int i = 0; i < grid_Row * grid_Col; i++)
            {
                string row = "";
                for (int j = 0; j < grid_Row * grid_Col; j++)
                {
                    cells[i, j].Reset();
                    row += cells[i, j].value + " ";
                }
                s += row + "\n";
            }
            //  Debug.Log(s);
        }
        private bool CheckWin()
        {
            if (useSuggest)
            {
                for (int i = 0; i < grid_Row * grid_Col; i++)
                {
                    for (int j = 0; j < grid_Row * grid_Col; j++)
                    {
                        if (cells[i, j].isIncorrect)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else {
                if (!selectCell.isOpen || selectCell.isIncorrect) return false;
                for (int i = 0; i < grid_Row * grid_Col; i++)
                {
                    for (int j = 0; j < grid_Row * grid_Col; j++)
                    {
                        if (cells[i, j].isIncorrect)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }            
        }
        private void CheckValueSame()
        {
            if (selectCell == null || selectCell.value == 0) return;
            int row = selectCell.row;
            int col = selectCell.col;
            int startRow = (row / typeRow) * typeRow;
            int startCol = (col / 3) * 3;
            //   string tes = "col: "+col+" :";
            for (int i = 0; i < grid_Row * grid_Col; i++)
            {
                //    tes += cells[i, col].value + " ";
                if ((i != row) && (cells[i, col].value == selectCell.value))
                {
                    cells[i, col].WrongValue();
                    selectCell.WrongValue();
                    selectCell.isIncorrect = true;
                }
                if ((i != col) && (cells[row, i].value == selectCell.value))
                {
                    cells[row, i].WrongValue();
                    selectCell.WrongValue();
                    selectCell.isIncorrect = true;
                }
                if ((((startRow + i / 3) != row || (startCol + i % 3) != col)) && (cells[startRow + i / 3, startCol + i % 3].value == selectCell.value))
                {
                    cells[startRow + i / 3, startCol + i % 3].WrongValue();
                    selectCell.WrongValue();
                    selectCell.isIncorrect = true;
                }
            }
            if (selectCell.value != levelAns[row, col])
            {
                PlaySound_FailBox();
                selectCell.WrongValue();
                selectCell.isIncorrect = true;
                return;
            }
            if (selectCell.value == levelAns[row, col])
            {
                PlaySound_ACBox();
                selectCell.isOpen = true;
                selectCell.isIncorrect = false;
                selectCell.Reset();                
                for (int i = 0; i < grid_Row * grid_Col; i++)
                {
                    cells[i, col].HightLight();
                    cells[row, i].HightLight();
                    cells[startRow + i / 3, startCol + i % 3].HightLight();
                }
                cells[row, col].Changecolor();
            }
            // Debug.Log(tes);
        }
        #endregion
        #region CameraFit
        [SerializeField] private GameObject khung;
        void CameraFit()
        {
            Resize(khung, 1.0f, "border");
        }
        private void Resize(GameObject gameObject, float ratio, string mode)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

            float worldScreenHeight = Camera.main.orthographicSize * 2f;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            float scaleX = worldScreenWidth * ratio / spriteRenderer.sprite.bounds.size.x;
            float scaleY = gameObject.transform.localScale.y;
            if (mode.Equals("border"))
            {
                scaleY = worldScreenHeight * ratio / spriteRenderer.sprite.bounds.size.y;
                // land.transform.localScale=new Vector3(scaleX*4,1f,1f);
                gameObject.GetComponent<SpriteRenderer>().size = new Vector2(scaleX, scaleY);
                scaleX = 1;
                scaleY = 1;
            }
            else
            {
                Debug.Log("Khong ton tai " + mode);
            }
            gameObject.transform.localScale = new Vector3(scaleX, scaleY, 1);
        }
        #endregion

        #region Xu li hoan lai buoc di truoc
        public struct PreviousStep
        {
            public int row, col;
            public int value;
        }
        public bool ReturnStep()
        {
            if (savePreviousStep.Count == 0)
            {
                return false;
            }
            int rowH = savePreviousStep[savePreviousStep.Count - 1].row;
            int colH = savePreviousStep[savePreviousStep.Count - 1].col;
            int valueH = savePreviousStep[savePreviousStep.Count - 1].value;
            savePreviousStep.RemoveAt(savePreviousStep.Count - 1);
            UI.Reset1Number(cells[rowH, colH].value);
            if (savePreviousStep.Count != 0)
            {
                int row = savePreviousStep[savePreviousStep.Count - 1].row;
                int col = savePreviousStep[savePreviousStep.Count - 1].col;
                int value = savePreviousStep[savePreviousStep.Count - 1].value;                
                ResetGird();
                if (rowH != row || colH != col)
                {
                    cells[rowH, colH].Init(0);
                    //     Debug.Log(12);
                    if (cells[row, col].isIncorrect == false)
                    {
                        cells[row, col].Changecolor();
                    }
                    else
                    {
                        cells[row, col].WrongValue();
                    }
                }
                else
                {
                    cells[row, col].WrongValue();
                    cells[row, col].UpdateValue(value);                
                }                
                int startRow = (row / typeRow) * typeRow;
                int startCol = (col / 3) * 3;
                for (int i = 0; i < grid_Row * grid_Col; i++)
                {
                    if ((i != row) && (cells[i, col].value == value))
                    {
                        cells[i, col].WrongValue();
                    }
                    else if (i != row)
                    {
                        cells[i, col].HightLight();
                    }
                    if ((i != col) && (cells[row, i].value == value)) { cells[row, i].WrongValue(); }
                    else if (i != col) { cells[row, i].HightLight(); };
                    if (((startRow + i / 3) != row || (startCol + i % 3) != col) && (cells[startRow + i / 3, startCol + i % 3].value == value))
                    {
                        cells[startRow + i / 3, startCol + i % 3].WrongValue();
                    }
                    else if ((startRow + i / 3) != row || (startCol + i % 3) != col)
                    {
                        cells[startRow + i / 3, startCol + i % 3].HightLight();
                    }
                }
               // selectCell = cells[row, col];
            }
            else
            {
                cells[rowH, colH].Init(0);
                // cells[row, col].Init(value);
            //    Debug.Log("aaaa");
                int startRow = (rowH / typeRow) * typeRow;
                int startCol = (colH / 3) * 3;
                for (int i = 0; i < grid_Row * grid_Col; i++)
                {
                    cells[i, colH].Reset();
                    cells[i, colH].HightLight();
                    cells[rowH, i].Reset();
                    cells[rowH, i].HightLight();
                    cells[startRow + i / 3, startCol + i % 3].Reset();
                    cells[startRow + i / 3, startCol + i % 3].HightLight();
                }
                cells[rowH, colH].Changecolor();
               // selectCell = cells[rowH, colH];
            }
            if (useNote == true)
            {
                DisplayNoteEnhance();
            }
            return true;
        }
        #endregion
        #region Ghi chu nang cao
        public bool useNote = false;
        private int[] CheckEnhance(int row, int col)
        {
            int startRow = (row / typeRow) * typeRow;
            int startCol = (col / 3) * 3;
            int[] arr = new int[grid_Row * grid_Col];
            for (int i = 0; i < grid_Row * grid_Col; i++)
            {
                arr[i] = -1;
            }
            for (int i = 0; i < grid_Row * grid_Col; i++)
            {
                //    tes += cells[i, col].value + " ";
                if (cells[i, col].isIncorrect == false && arr[cells[i, col].value - 1] == -1)
                {
                    arr[cells[i, col].value - 1] = cells[i, col].value - 1;
                }
                if (cells[row, i].isIncorrect == false && arr[cells[row, i].value - 1] == -1)
                {
                    arr[cells[row, i].value - 1] = cells[row, i].value - 1;
                }
                if (cells[startRow + i / 3, startCol + i % 3].isIncorrect == false && arr[cells[startRow + i / 3, startCol + i % 3].value - 1] == -1)
                {
                    arr[cells[startRow + i / 3, startCol + i % 3].value - 1] = cells[startRow + i / 3, startCol + i % 3].value - 1;
                }
            }
            //string s = "";
            //Debug.Log(row + " " + col);
            //for (int i = 0; i < grid_Row * grid_Col; i++)
            //{
            //    s += arr[i] + " ";
            //}
            //    Debug.Log(s);
            return arr;
        }
        public void DisplayNoteEnhance()
        {
            useNote = true;
            for (int i = 0; i < grid_Row * grid_Col; i++)
            {
                for (int j = 0; j < grid_Row * grid_Col; j++)
                {
                    if (cells[i, j].isIncorrect == true) {
                        cells[i, j].suggest = new int[9] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };
                        int[] arr = new int[grid_Row * grid_Col];
                        arr = CheckEnhance(i, j);
                        cells[i, j].suggest = arr;
                        cells[i, j].DisplayNote(arr);
                    }
                }
            }
            //  Invoke("CancelEhance", 5);
        }
        public void CancelEhance()
        {
            
            for (int i = 0; i < grid_Row * grid_Col; i++)
            {
                for (int j = 0; j < grid_Row * grid_Col; j++)
                {
                    cells[i, j].CancelDisplayNote();
                }
            }
        }
        #endregion
        #region Goi y
        //1 o chi con lai chinh no 
        private bool useSuggest = false;
        public void OnDisPlaySuggest()
        {
            useSuggest = true;
            int row, col;
            (row,col)  = OnlyACell();
         //.   Debug.Log(row + " " + col);
            if(row!=-1 && col != -1)
            {
                cells[row, col].Init(levelAns[row, col]);
                ResetGird();
                int startRow = (row / typeRow) * typeRow;
                int startCol = (col / 3) * 3;
                for (int i = 0; i < grid_Row * grid_Col; i++)
                {
                  //  cells[i, col].Reset();
                    cells[i, col].HightLight();
                   // cells[row, i].Reset();
                    cells[row, i].HightLight();
                 //   cells[startRow + i / 3, startCol + i % 3].Reset();
                    cells[startRow + i / 3, startCol + i % 3].HightLight();
                }
                cells[row, col].Changecolor();    
                
                PreviousStep previous = new PreviousStep();
                previous.row = row; previous.col = col;
                previous.value = levelAns[row, col];
                savePreviousStep.Add(previous);
            //    selectCell = cells[row, col];
                Effec1RowOrCol(row, col);
                if (CheckWin())
                {
                    Invoke("AppearUIWin", 1f);
                }
            }
            else
            {
                (row, col) = IsCellInRowAndCol();
                if(row!=-1 && col != -1)
                {
                    cells[row, col].Init(levelAns[row, col]);
                    ResetGird();
                    int startRow = (row / typeRow) * typeRow;
                    int startCol = (col / 3) * 3;
                    for (int i = 0; i < grid_Row * grid_Col; i++)
                    {
                      //  cells[i, col].Reset();
                        cells[i, col].HightLight();
                      //  cells[row, i].Reset();
                        cells[row, i].HightLight();
                      //  cells[startRow + i / 3, startCol + i % 3].Reset();
                        cells[startRow + i / 3, startCol + i % 3].HightLight();
                    }
                    cells[row, col].Changecolor();                    
                    PreviousStep previous = new PreviousStep();
                    previous.row = row; previous.col = col;
                    previous.value = levelAns[row, col];
                    savePreviousStep.Add(previous);
                   // selectCell = cells[row, col];
                    Effec1RowOrCol(row, col);
                    if (CheckWin())
                    {
                        Invoke("AppearUIWin", 1f);
                    }
                }

            }
            if (useNote == true)
            {
                CancelEhance();               
                DisplayNoteEnhance();
            }
            //Effec1RowOrCol(row, col);
            //   Debug.Log(row+" "+ col);
           // CancelEhance();
        }
        private (int, int) OnlyACell()
        {
            for (int i = 0; i < grid_Row * grid_Col; i++)
            {
                for (int j = 0; j < grid_Row * grid_Col; j++)
                {
                    if (cells[i, j].isIncorrect == true)
                    {
                        cells[i, j].suggest= new int[9] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };
                        int[] arr = new int[grid_Row * grid_Col];
                        arr = CheckEnhance(i, j);
                        int cnt = 0;
                        cells[i, j].suggest = arr;
                        for(int k = 0; k < grid_Row * grid_Col; k++)
                        {
                            if (arr[k]==-1)
                            {
                                cnt++;
                            }
                        }
                        if (cnt == 1)
                        {
                            return (i, j);
                        }
                    }
                }
            }
            return (-1,-1);
        }
        public int [] CheckInRowAndCol(int row,int col)
        {
            int startRow = (row / typeRow) * typeRow;
            int startCol = (col / 3) * 3;
            int[] arr = new int[grid_Row * grid_Col];
            for (int i = 0; i < grid_Row * grid_Col; i++)
            {
                arr[i] = -1;
            }
            for (int i = 0; i < grid_Row * grid_Col; i++)
            {
                //    tes += cells[i, col].value + " ";
                if (cells[i, col].isIncorrect == false && arr[cells[i, col].value - 1] == -1)
                {
                    arr[cells[i, col].value - 1] = cells[i, col].value - 1;
                    for(int j = 0; j < grid_Row * grid_Col; j++)
                    {
                        if (cells[i, col].suggest[j] != -1)
                        {
                            if (arr[cells[i, col].suggest[j]] == -1)
                            {
                                arr[cells[i, col].suggest[j]] = cells[i, col].suggest[j];
                            }
                        }
                    }
                }
                if (cells[row, i].isIncorrect == false && arr[cells[row, i].value - 1] == -1)
                {
                    arr[cells[row, i].value - 1] = cells[row, i].value - 1;
                    for (int j = 0; j < grid_Row * grid_Col; j++)
                    {
                        if(cells[row, i].suggest[j] != -1){
                            if (arr[cells[row, i].suggest[j]] == -1)
                            {
                                arr[cells[row, i].suggest[j]] = cells[row, i].suggest[j];
                            }
                        }                       
                    }
                }
                if (cells[startRow + i / 3, startCol + i % 3].isIncorrect == false && arr[cells[startRow + i / 3, startCol + i % 3].value - 1] == -1)
                {
                    arr[cells[startRow + i / 3, startCol + i % 3].value - 1] = cells[startRow + i / 3, startCol + i % 3].value - 1;
                    for (int j = 0; j < grid_Row * grid_Col; j++)
                    {
                        if (cells[startRow + i / 3, startCol + i % 3].suggest[j] != -1)
                        {
                            if (arr[cells[startRow + i / 3, startCol + i % 3].suggest[j]] == -1)
                            {

                                arr[cells[startRow + i / 3, startCol + i % 3].suggest[j]] = cells[startRow + i / 3, startCol + i % 3].suggest[j];
                            }
                        }
                            
                    }
                }
            }
            return arr;
        }
        private struct Candidate
        {
            public int row;
            public int col;
            public int val;

            public Candidate(int row, int col, int val)
            {
                this.row = row;
                this.col = col;
                this.val = val;
            }
        }
        private (int,int) IsCellInRowAndCol()
        {
            List<Candidate> candidates = new List<Candidate>();
            for (int i = 0; i < grid_Row * grid_Col; i++)
            {
                for (int j = 0; j < grid_Row * grid_Col; j++)
                {
                    if (cells[i, j].isIncorrect == true)
                    {
                        int[] arr = new int[grid_Row * grid_Col];
                        arr = CheckInRowAndCol(i, j);
                        int cnt = 0;
                       // string s = "";
                        for (int k = 0; k < grid_Row * grid_Col; k++)
                        {
                            if (arr[k] == -1)
                            {
                                cnt++;
                            }
                         //  s+= arr[k]+" ";
                        }
                        cells[i, j].candidate = cnt;                      
                        candidates.Add(new Candidate( i, j, cnt));
                        //if (cnt == 1)
                        //{
                        //  return (i, j);
                        //}
                    //    Debug.Log(s);
                    }
                }
            }
            if(candidates!=null)
            {
                List<Candidate> sortedCandidates = candidates.OrderBy(candidate => candidate.val).ToList();
                if(sortedCandidates.Count > 0)
                {
                    Candidate candidate = sortedCandidates[0];
                    return (candidate.row, candidate.col);
                }
                return (-1, -1);
            }
            else
            {
                return (-1, -1);
            }
        }
        #endregion
        #region Sound
        public AudioClip win;
        public AudioClip lose;
        public AudioClip box;
        public AudioClip clickBox;
        public AudioClip failBox;
        public AudioClip acBox;
        public void PlaySound_ACBox()
        {
            /*SoundManager.instance.PlaySfx(acBox);
            SoundManager.instance.PlayBtnClick();*/
        }
        public void PlaySound_FailBox()
        {
            /*SoundManager.instance.PlaySfx(failBox);
            SoundManager.instance.PlayBtnClick();*/
        }
        public void PlaySound_Box()
        {
            /*SoundManager.instance.PlaySfx(box);
            SoundManager.instance.PlayBtnClick();*/
        }
        public void PlaySound_Win()
        {
            /*SoundManager.instance.PlaySfx(win);
            SoundManager.instance.PlayBtnClick();*/
        }
        public void PlaySound_lose()
        {
            /*SoundManager.instance.PlaySfx(lose);
            SoundManager.instance.PlayBtnClick();*/
        }
        public void PlaySound_ClickBox()
        {
            /*SoundManager.instance.PlaySfx(clickBox);
            SoundManager.instance.PlayBtnClick();*/
        }
        #endregion
        #region Win
        [SerializeField] private ParticleSystem effectWin;
        void AppearUIWin()
        {
            StartCoroutine(DelayWin());
        }
        IEnumerator DelayWin()
        {
            isPlaying = false;
            PlaySound_Win();
            effectWin.Play();
            UI.WinTime();
            yield return new WaitForSeconds(1f);
            //effectWin.Play();
            for (int row = 0; row < grid_Row * grid_Col; row++)
            {
                for (int col = 0; col < grid_Col * grid_Row; col++)
                {
                    cells[row, col].effect.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f);
                    cells[row, col].effect.color = new Color(1, 1, 1, 1);
                    cells[row, col].imageParent.SetActive(false);
                    cells[row, col].effect.DOFade(0f, 1f);
                }
                yield return new WaitForSeconds(0.5f / (typeRow * 3));
            }
            yield return new WaitForSeconds(1f);
            // effectWin.SetActive(false);
            //DataManager.instance.waterDrop += UI.amountCoin;
            //Duc.UIEndGame2.instance.DisplayPanelEndGame(true, 1, UI.amountCoin, false, true);
            effectWin.Stop();
        }
        #endregion
    }
}