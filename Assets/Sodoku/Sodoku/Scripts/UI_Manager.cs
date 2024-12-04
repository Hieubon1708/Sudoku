
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Soduku
{
    public class UI_Manager : MonoBehaviour
    {
        int saveMap=6;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private Transform parentNumbers;

        [SerializeField] private TextMeshProUGUI healthTxt;
        [SerializeField] private TextMeshProUGUI undoTxt;
        [SerializeField] private TextMeshProUGUI noteTxt;
        [SerializeField] private TextMeshProUGUI suggestTxt;
        private int currentHealth;
        public int amountCoin;

        private void Awake()
        {
            //coinText.text = DataManager.instance.waterDrop.ToString();
            ResetAttribute();
        }
        private void Start()
        {
            maxPage = contentPageTfrm.childCount;
            pos = new float[maxPage];
            scrollValue=new float[maxPage];
            distace = 1f / (pos.Length - 1f);
            Debug.Log($"length {pos.Length} + maxPage {maxPage}");
            for (int i = 0; i < maxPage; i++)
            {
                scrollValue[i] = distace * i;
            }
            SetScrollBarValue(0);
        }
        public void ClickTutorial()
        {
            tutorial = !tutorial;

        }
        //Swip Page
        [Header("Swip Page")]
        public Scrollbar scrollbar;
        public Transform contentPageTfrm;
        float[] pos;
        public float[] scrollValue;
        public bool checkTime = false;
        public bool tutorial = false;
        float distace = 1f;
        int currentPage = 0;
        int maxPage = 0;
        private bool isSwipeMode = false;
        private float swipeTime = 0.2f;
        private float startTouchX;              
        private float endTouchX;
        private void SetScrollBarValue(int index)
        {
            currentPage = index;
            scrollbar.value = scrollValue[index];
        }
        private void Update()
        {
            if (checkTime == true)
            {
                DisPlayTime();
            }
            if (tutorial == false)
            {
                return;
            }
            if (isSwipeMode == true) return;

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                startTouchX = Input.mousePosition.x;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                endTouchX = Input.mousePosition.x;
               Swipe();
            }
#endif
#if UNITY_ANDROID
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                { 
                    startTouchX = touch.position.x;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    endTouchX = touch.position.x;

                    Swipe();
                }
            }
#endif

        }
        private void Swipe()
        {
            if (Mathf.Abs(startTouchX - endTouchX) < distace)
            {
                StartCoroutine(OnSwipeOneStep(currentPage));
                return;
            }
            bool isLeft = startTouchX < endTouchX ? true : false;
            if (isLeft)
            {
                if (currentPage == 0) return;
                currentPage--;
            }
            else
            {
                if (currentPage == maxPage - 1) return;
                currentPage++;
            }
            StartCoroutine(OnSwipeOneStep(currentPage));
        }
        private IEnumerator OnSwipeOneStep(int index)
        {
            float start = scrollbar.value;
            float current = 0;
            float percent = 0;

           isSwipeMode = true;

            while (percent < 1)
            {
                current += Time.deltaTime;
                percent = current / swipeTime;

                scrollbar.value = Mathf.Lerp(start, scrollValue[index], percent);
                yield return null;
            }

           isSwipeMode = false;
        }

        private void ResetAttribute()
        {
            checkTime = false;
           /* DataManager.instance.Undo = 3;
            DataManager.instance.Refresh = 3;
            DataManager.instance.Hint = 3;
            undoTxt.text = DataManager.instance.Undo.ToString();
            noteTxt.text = DataManager.instance.Refresh.ToString();
            suggestTxt.text = DataManager.instance.Hint.ToString();
            coinText.text = DataManager.instance.waterDrop.ToString();*/
         
        }
        public int CurrentHealth
        {
            get {  return currentHealth ; }
            set
            {
                currentHealth = value;
                string text = healthTxt.text.ToString().Trim().Substring(1);
                healthTxt.text = currentHealth.ToString()+text;
                if (currentHealth <= 0)
                {
                    GamePlay.instance.PlaySound_lose();
                    GamePlay.instance.isPlaying = false;
                    Invoke("ShowRevive", 0.5f);
                }
            }
        }
        #region LOSE
        [Header("Attribute")]
        public GameObject panelLose;
        [SerializeField] private TextMeshProUGUI txtCoolDown;
        private Tween tweenCoolDown;
        public GameObject btnLose;
        public Image imgCoolDown;
        public void ShowRevive()
        {
            if(tweenCoolDown != null) tweenCoolDown.Kill();
            int time = 15;
            imgCoolDown.DOKill();
            btnLose.SetActive(false);
            imgCoolDown.fillAmount = 1f;
            txtCoolDown.text = time.ToString();
            checkTime = false;
            tweenCoolDown = DOVirtual.DelayedCall(1, delegate
            {
                time--;
                txtCoolDown.text = time.ToString();
                
                if (time == 10f)
                {
                    btnLose.SetActive(true);
                }
            }).SetLoops(15).SetEase(Ease.Linear).OnComplete(delegate { CloseRevive(); });
            imgCoolDown.DOFillAmount(0, 15).SetEase(Ease.Linear);
            panelLose.SetActive(true);
        }
        public void CloseRevive()
        {
            if (panelLose.activeInHierarchy)
            {
                panelLose.SetActive(false);
                ShowLose();
            }
        }
        public void WinTime()
        {
            float maxTime = PlayerPrefs.GetFloat("bestTime", 0f);
            if (maxTime > timeInGame)
            {
                PlayerPrefs.SetFloat("bestTime", timeInGame);
            }
            checkTime = false;
        }
        public void ShowLose()
        {
            //Duc.UIEndGame2.instance.DisplayPanelEndGame(true, 2, 0);
        }
        #endregion
        public void Continue()
        {
            timeInGame = 0f;
            for (int i = 0; i < GamePlay.instance.gridParent.transform.childCount; i++)
            {
                Destroy(GamePlay.instance.gridParent.transform.GetChild(i).gameObject);
            }
            int num = parentNumbers.childCount;
            for (int i = 0; i < parentNumbers.childCount; i++)
            {
                Destroy(parentNumbers.GetChild(i).gameObject);
                dicNumber[i + 1] = num;
            }
            GamePlay.instance.ChooseDifficulty(saveMap);
            if (saveMap == 0) CurrentHealth = 1;
            else CurrentHealth = 3;
            ResetAttribute();
            ResetMap();
            GamePlay.instance.useNote = false;
            checkTime = true;
            GamePlay.instance.isPlaying = true;
            //  Debug.Log("Da va vao");
        }
        public void RePlay()
        {

            //SoundManager.instance.PlayBtnClick();
            CurrentHealth = 1;

            //   ResetAttribute();
            GamePlay.instance.CancelEhance();
            checkTime = true;
            if (saveMap == 0)
            {
                for(int value = 1; value <= 6; value++)
                {
                    if (dicNumber[value] <= 0)
                    {
                        parentNumbers.GetChild(value - 1).gameObject.SetActive(true);
                    }
                    dicNumber[value] = 6;
                }
            }
            else
            {
                for (int value = 1; value <= 9; value++)
                {
                    if (dicNumber[value] <= 0)
                    {
                        parentNumbers.GetChild(value - 1).gameObject.SetActive(true);
                    }
                    dicNumber[value] = 9;
                }
            }
            timeInGame = 0f;
            GamePlay.instance.isPlaying = true;
            GamePlay.instance.useNote = false;
            GamePlay.instance.ResetMap();
        }
        public void Refresh()
        {
            //SoundManager.instance.PlayBtnClick();
            if (currentHealth < 1)
            {
                CurrentHealth = 1;
            }
            GamePlay.instance.isPlaying = true;
            checkTime = true;

            GamePlay.instance.ReturnStep();


        }
        public void Sound_BtnClick()
        {
            //SoundManager.instance.PlayBtnClick();
        }
        public void Home()
        {
            //SoundManager.instance.PlayBtnClick();
            for (int i = 0; i < GamePlay.instance.gridParent.transform.childCount; i++)
            {
                Destroy(GamePlay.instance.gridParent.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < parentNumbers.childCount; i++)
            {
                Destroy(parentNumbers.GetChild(i).gameObject);
            }
            if (saveMap == 0) CurrentHealth = 1;
            else CurrentHealth = 3;
            timeInGame = 0f;
            checkTime = false;
            txtTime.text = "00:00";
            GamePlay.instance.useNote = false;
            ResetMap();
        }
        [SerializeField] Transform parentNote;
        Dictionary<int, int> dicNumber;
        public void ClickType(int i)
        {
            //SoundManager.instance.PlayBtnClick();
            dicNumber = new Dictionary<int, int>();
            ResetAttribute();
            if (i == 0)
            {
           //     parentNote.GetChild(1).gameObject.SetActive(true);
                currentHealth = 1;
                healthTxt.text= currentHealth.ToString() + "/1";
                amountCoin = 6;
                for(int j = 0; j <= 6; j++)
                {
                    dicNumber[j] = 6;

                }
                maxTime = 2f;
            }
            else
            {
                currentHealth = 3;
            //    parentNote.GetChild(1).gameObject.SetActive(true);
                healthTxt.text = currentHealth.ToString() + "/3";
                for (int j = 0; j <= 9; j++)
                {
                    dicNumber[j] = 9;
                }
            }
            if(i==1) { amountCoin = 15;maxTime = 5f; }
            if (i == 2) { amountCoin = 25; maxTime = 10f; }
            if(i == 3) {  amountCoin =40;maxTime = 20f; }
            GamePlay.instance.ChooseDifficulty(i);
            saveMap = i;
            checkTime = true;
            GamePlay.instance.isPlaying = true;
            //Debug.Log(timeInGame);
            
            //  StartCoroutine(DelayAppear(i));
        }
        public void Destroy1Number(int value)
        {
            //Debug.Log("-------------value-==============" + value + "dic" + dicNumber[value]);
            if (value != 0)
            {
                dicNumber[value] -= 1;
                if (dicNumber[value] <= 0)
                {
                   // Debug.Log("value " + value);
                    parentNumbers.GetChild(value - 1).gameObject.SetActive(false);
                }
            }           
        }
        public void Reset1Number(int value)
        {
            if (value != 0)
            {
                if (dicNumber[value] <= 0)
                {
                    parentNumbers.GetChild(value - 1).gameObject.SetActive(true);
                }
                dicNumber[value] += 1;
            }           
            
        }
        public void StopRepeating()
        {
            CancelInvoke("DisPlayTime");
        }

        public void ReturnStep()
        {
            if (GamePlay.instance.isPlaying == false) return;
            /*SoundManager.instance.PlayBtnClick();
            if (DataManager.instance.Undo >= 1)
            {
                if (GamePlay.instance.ReturnStep() == true)
                {
                    DataManager.instance.Undo -= 1;
                    undoTxt.text = DataManager.instance.Undo.ToString();
                    if (DataManager.instance.Undo < 1)
                    {
                        ADS(0);
                    }
                }  
                
            }*/
       //     Debug.Log("step");
        }
        public void Note()
        {
            if (GamePlay.instance.isPlaying == false) return;
            /*SoundManager.instance.PlayBtnClick();
            if (DataManager.instance.Refresh>=1)
            {
                if (GamePlay.instance.useNote == true)
                {
                    return;
                }
                DataManager.instance.Refresh -= 1;
                GamePlay.instance.CancelEhance();
                GamePlay.instance.DisplayNoteEnhance();
                noteTxt.text = DataManager.instance.Refresh.ToString();
                if (DataManager.instance.Refresh < 1)
                {
                    ADS(2);
                }
            }*/
        }
        public void Suggest()
        {
            /*if (GamePlay.instance.isPlaying == false) return;
            SoundManager.instance.PlayBtnClick();
            if (DataManager.instance.Hint >= 1)
            {
                DataManager.instance.Hint -= 1;
                GamePlay.instance.OnDisPlaySuggest();
                suggestTxt.text = DataManager.instance.Hint.ToString();
                if (DataManager.instance.Hint < 1)
                {
                    ADS(1);
                }
            }     */       
        }
        public GameObject[] btnAds;
        public GameObject[] imgUse;
        public void ADS(int index)
        {
            /*if (index == 0)
            {
                DataManager.instance.Undo += 1;              
            }
            else if (index == 2)
            {
                DataManager.instance.Refresh += 1;
            }
            else
            {
                DataManager.instance.Hint += 1;
            }*/
            btnAds[index].SetActive(true);
            imgUse[index].SetActive(false);
        }
        public void ResetMap()
        {
            for(int index=0;index<3;index++)
            {
                btnAds[index].SetActive(false);
                imgUse[index].SetActive(true);
            }
        }
        public void ResetNotUseAds(int index)
        {
            btnAds[index].SetActive(false);
            imgUse[index].SetActive(true);
        }
        //public void ClickNext(int index)
        //{
        //    SoundManager.instance.PlayBtnClick();
        //    if (index == 0)
        //    {
        //        DataManager.instance.Undo += 1;
        //        undoTxt.text = DataManager.instance.Undo.ToString();
        //    }
        //    else if (index == 2)
        //    {
        //        DataManager.instance.Refresh += 1;
        //        noteTxt.text = DataManager.instance.Refresh.ToString();
        //    }
        //    else
        //    {
        //        DataManager.instance.Hint += 1;
        //        suggestTxt.text = DataManager.instance.Hint.ToString();
        //    }
        //    //ResetNotUseAds(index);
        //}
        #region Time in Game
        [Header("Time In Game")]
        public float timeInGame = 0f;
        private float maxTime = 2f;
        public TextMeshProUGUI txtTime;      
        private void DisPlayTime()
        {
            timeInGame += Time.deltaTime;
            int minutes = Mathf.FloorToInt(timeInGame / 60);
            int seconds = Mathf.FloorToInt(timeInGame % 60);           
            txtTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        #endregion
    }

}
