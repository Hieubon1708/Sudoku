using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour 
{
    public static gameManager instance;

    public Color colorSelected;
    public Image imagePrevious;
    public numpadController numpad;
    public checkButtonController checkButton;
    public arenaManager arena;
    public EndGameController endGamePanel;

    public float timer;
    public int moves;

    public bool randomGame;
    public bool countTime;

    public audioController audio;

    public TextMeshProUGUI timerTxt;
    public TextMeshProUGUI movesTxt;

    public void ColorSet(Image image)
    {
        if(imagePrevious != null) imagePrevious.color = Color.white;
        imagePrevious = image;
        image.color = colorSelected;
    }
    
	void Awake()
    {
        instance = this;

        timer = 0F;
        moves = 0;

        /*timerTxt = GameObject.FindGameObjectWithTag("Timer display").gameObject.GetComponent<Text>();
        movesTxt = GameObject.FindGameObjectWithTag("Moves display").gameObject.GetComponent<Text>();*/
        
        //checkButton.deactivate();

        LevelManager.Load();
        arena.Init();
    }
	
	void Update () 
    {
        if (countTime)
        {
            timer += Time.deltaTime;
            timerTxt.text = "TIME: " + transformToTime(timer);
            movesTxt.text = "MOVES: " + moves.ToString();
        }

        /*if (arena.checkEmpty()) checkButton.activate();
        else checkButton.deactivate();*/
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            arena.setActive(false);
            MainMenuManager.mainMenu.setActive(true);
        }
	}

    public string transformToTime(float time = 0)
    {
        if (time == 0) time = timer;
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void resetTimer()
    {
        countTime = true;
        timer = 0F;
    }

    public void checkSudoku()
    {
        if (!arena.checkHorizontal() || !arena.checkVertical() || !arena.checkSquare())
        {
            Debug.Log("--- Sudoku is incorrect ---");
            audio.play(audioController.soundType.SOUND_BADSUDOKU);
        }
        else
        {
            Debug.Log("--- SUDOKU IS CORRECT ---");
            audio.play(audioController.soundType.SOUND_GOODSUDOKU);
            endGamePanel.setActive(true);
            endGamePanel.refresh(arena.getActiveLevel());
        }         
    }
}
