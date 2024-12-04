using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Soduku
{
    public class Cell : MonoBehaviour
    {
        [Header("Atrribute")]
        public int value;
        public int row;
        public int col;
        public bool isOpen;
        public bool isIncorrect;
        [SerializeField] Image bg;
        public Image effect;
        [SerializeField] private TextMeshProUGUI valueText;
        [Header("HighLighted")]
        [SerializeField] private Color selectCell;
        [SerializeField] private Color wrongCell;
        [SerializeField] private Color checkCell;
        [SerializeField] private Color defaultCell;
        [SerializeField] public Image sprBG;
        [SerializeField] public GameObject imageParent;
        [SerializeField] public Sprite selectImage;
        [SerializeField] public Sprite wrongImage;
        [SerializeField] public Sprite checkImage;

        [Header("Ghi chu")]
        [SerializeField] private Transform ghiChu;
        [Header("Goi y")]
        public int[] suggest;
        public int candidate;
        private void Awake()
        {
            suggest = new int[9] { -1,-1,-1,-1,-1,-1,-1,-1,-1};
            candidate = 0;
        }
        public void Init(int value)
        {
            isIncorrect = true ;
            this.value = value;
            if (value == 0)
            {
                isOpen = false;
                valueText.text= string.Empty;

            }
            else
            {
                isOpen = true;
                isIncorrect = false;
                valueText.text=value.ToString();    
            }
        }
        public void HightLight()
        {
          //  bg.color = checkCell;
            bg.sprite = checkImage;
            imageParent.SetActive(true);
        }
        public void Changecolor()
        {
            // bg.color= selectCell;
            bg.sprite = selectImage;
            imageParent.SetActive(true);
            if (!isIncorrect && value != 0)
            {                
                valueText.color = defaultCell;
            }
        }
        public void Select()
        {   
           GamePlay.instance.SelectCell(row, col);
        }
        public void UpdateValue(int value)
        {

            this.value = value;
            valueText.text=value==0?"":this.value.ToString();
        }
        public void WrongValue()
        {
            //  bg.color = wrongCell;
            bg.sprite = wrongImage;
            imageParent.SetActive(true);
            valueText.color = Color.red;
        }
        public void Reset()
        {
            //bg.color = new Color(1,1,1,1);        
            imageParent.SetActive(false);
            if (!isIncorrect && value != 0)
            {
                valueText.color = defaultCell;
            }
        }
        public void DisplayNote(int[]arr)
        {
           // if (isIncorrect == false) return;
            for(int i = 0; i < ghiChu.childCount; i++)
            {
                if (arr[i] == -1)
                {
                    ghiChu.GetChild(i).gameObject.SetActive(true);
                }
            }
        }         
        public void CancelDisplayNote()
        {
            
            for (int i = 0; i < ghiChu.childCount; i++)
            {
                ghiChu.GetChild(i).gameObject.SetActive(false);
                CancelDisPlayNumberInNote(i);
            }
        }
        public void DisplayNumberInNote(int val)
        {
          //  Debug.Log($"display row {row} col {col} val {val}");
            ghiChu.GetChild(val).GetChild(0).gameObject.SetActive(true);
        }
        public void CancelDisPlayNumberInNote(int val)
        {
          //  Debug.Log($"cancel row {row} col {col} val {val}");
            ghiChu.GetChild(val).GetChild(0).gameObject.SetActive(false);
        }
        public void ResetNumber()
        {
            for (int i = 0; i < ghiChu.childCount; i++)
            {
                CancelDisPlayNumberInNote(i);
            }
        }
        private void OnDestroy()
        {
            transform.DOKill();
            effect.DOKill();
        }
    }
  
}