using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soduku
{
    public class Generator
    {
        public enum DifficultyLevel
        {
            easy,medium,hard
        }
        //typeAmount = 3x2, 3x3
        public int[,] grid;

        #region Tao so ngau nhien khi bat dau game
        public int[,] CreateLevel(int typeAmout, int typeRow)
        {
          //  Debug.Log(typeRow);
            grid = new int[typeAmout, typeAmout];
            InitGrid(typeAmout,typeRow);
            return grid;
        }
        public int[,] MaskBox(int typeAmout, int typeRow,int clickType) {
            int emeptyBox = 0;
            if (clickType==0)
            {
                emeptyBox = UnityEngine.Random.Range(15, 20);
            }
            else if(clickType==1) 
            {
                emeptyBox = UnityEngine.Random.Range(38, 45);
            }
            else if (clickType==2) 
            {
                //   emeptyBox = UnityEngine.Random.Range(55, 65);
                emeptyBox = UnityEngine.Random.Range(50, 55);
            }
            else
            {
                emeptyBox= UnityEngine.Random.Range(55, 65);
            }
            if(emeptyBox >= 55)
            {
                emeptyBox = 81 - emeptyBox;
            }
            //Kho tu 50 den 55
            //chuyen gia tu  60 den 65
            string s = "";
            //Debug.Log(emeptyBox);
            int[,] arr = new int[typeAmout, typeAmout];
            Array.Copy(grid, arr, arr.Length);
            while (emeptyBox > 0)
            {
                int randRow = UnityEngine.Random.Range(0, typeAmout);
                int randCol = UnityEngine.Random.Range(0, typeAmout);
                if (arr[randRow, randCol] != 0)
                {
                    int tmp = arr[randRow, randCol];
                    arr[randRow, randCol] = 0;
                    if (CheckSolution(arr, typeRow, typeAmout))
                    {
                        emeptyBox--;
                        s+=randRow + " " + randCol+"\n";
                    }
                    else
                    {
                        arr[randRow, randCol] = tmp;
                    }
                }
            }
           
            Debug.Log(s);
            string testS = "";
            int dem = 0;
            for (int i = 0; i < typeAmout; i++)
            {
                string row = "";
                for (int j = 0; j < typeAmout; j++)
                {
                    row += arr[i, j] + " ";
                    if (arr[i, j] == 0)
                    {
                        dem++;
                    }
                }
                testS += row + "\n";
            }
            //Debug.Log("Grid: "+dem);
           Debug.Log(testS);
            return arr;
        }
        private void InitGrid(int typeAmount,int typeRow)
        {
            List<int> numbers = new List<int>();
            for (int i = 0; i < typeAmount; i++)
            {
                numbers.Add(i+1);
            }
            Shuffle(numbers);
            for (int i = 0; i < typeAmount; i++)
            {
                grid[0,i] = numbers[i];
            }
            FillGrid(1, 0, typeAmount,typeRow);
        }  
        private  bool FillGrid(int r,int c,int typeAmount,int typeRow)
        {
            if (r == typeAmount)
            {
                return true;
            }
            if(c == typeAmount)
            {
                return FillGrid(r+1,0,typeAmount,typeRow);
            }
            List<int> numbers = new List<int>();
            for (int i = 0; i < typeAmount; i++)
            {
                numbers.Add(i + 1);
            }
            Shuffle(numbers);
            foreach (var num in numbers)
            {
                if (IsVal(num, r, c, grid, typeAmount,typeRow))
                {
                    grid[r, c] = num;
                    if (FillGrid(r, c+1, typeAmount,typeRow))
                    {
                        return true;
                    }
                }
            }
            grid[r,c] = 0;
            return false;
        }
        #endregion


        #region Ham Kiem tra dieu kien thoa man
        //Kiem tra xem co thoa man hang cheo ngang doc co ton tai so hay chua
        private bool IsVal(int value,int row,int col, int[,] board,int typeAmount,int typeRow)
        {
            //Hang ngang co so do hay chua
            for(int i=0;i<typeAmount;i++)
            {
                if (board[row,i] == value)
                {
                    return false;
                }
            }
            for(int i = 0; i < typeAmount; i++)
            {
                if (board[i,col] == value)
                {
                    return false;
                }
            }
            int startRow = (row / typeRow) * typeRow;
            int startCol= (col / 3) * 3;
            for(int r = startRow; r < typeRow+startRow; r++)
            {
                for(int c= startCol; c < 3+startCol;c++)
                {
                    if(board[r,c] == value) { return false; }
                }
            }
            return true;
        }
        private void Shuffle(List<int> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n);
                int temp = list[k];
                list[k] = list[n];
                list[n] = temp;
            }
        }
        #endregion


        public bool CheckSolution(int[,] board,int typeRow,int len)
        {
            int cnt = 0;
            int row=-1,col=-1;
           // Debug.Log(len);
            for(int r = 0; r < len; r++)
            {
                for(int c = 0; c < len; c++)
                {
                    if (board[r,c]==0)
                    {
                        row = r;
                        col = c;
                        break;
                    }
                }
                if(row!=-1)
                {
                    break;
                }
            }
            if (row == -1)
            {
                return true;
            }
            for(int val = len; val >=1; val--)
            {
                if(IsVal(val, row, col,board, len,typeRow))
                {
                    board[row, col] = val;
                    if(CheckSolution(board,typeRow,len))
                    {
                        cnt++;
                    }
                    if (cnt > 1)
                    {
                         board[row, col] = 0;
                        return false;
                    }
                    board[row, col] = 0;
                }
            }
            return cnt == 1;
        }
        private bool BackTrack(int[,]board,int row,int col,int len,int typeRow)
        {
            if (row == len)
            {
                return true;
            }
            if(col== len)
            {
                return BackTrack(board, row + 1, 0,len,typeRow);
            }
            if (board[row, col] != 0) { 
                return BackTrack(board, 0,len,len,typeRow);
            }
            for(int i = 1; i <= len; i++)
            {
                if (IsVal(i, row, col, board, len, typeRow)){
                    board[row, col] = i;
                    if (BackTrack(board, row, col + 1, len, typeRow))
                    {
                        return true;
                    }
                    board[row,col+1] = 0;
                }
            }
            return false;
        }
    }
}