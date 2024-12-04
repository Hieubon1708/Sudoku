using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndGameController : MonoBehaviour 
{
    public bool active;

    private Text timeValue;
    private Text timeValueLast;
    private Text movesValue;
    private Text movesValueLast;

    private Level activeLevel;

    void Awake()
    {
        timeValue = gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).GetComponent<Text>();
        movesValue = gameObject.transform.GetChild(3).gameObject.transform.GetChild(0).GetComponent<Text>();
        timeValueLast = gameObject.transform.GetChild(4).gameObject.transform.GetChild(0).GetComponent<Text>();
        movesValueLast = gameObject.transform.GetChild(5).gameObject.transform.GetChild(0).GetComponent<Text>();
    }

    public void setActive(bool state)
    {
        gameObject.SetActive(state);
        active = state;
    }

    public void refresh(Level lev)
    {
        gameManager.instance.countTime = false;
        activeLevel = lev;

        timeValue.text = gameManager.instance.transformToTime();
        movesValue.text = gameManager.instance.moves.ToString();

        if(!gameManager.instance.randomGame)
        {
            timeValueLast.transform.parent.gameObject.SetActive(true);
            movesValueLast.transform.parent.gameObject.SetActive(true);
            timeValueLast.text = gameManager.instance.transformToTime(lev.bestTime);
            movesValueLast.text = lev.bestMoves.ToString();
        }
        else
        {
            timeValueLast.transform.parent.gameObject.SetActive(false);
            movesValueLast.transform.parent.gameObject.SetActive(false);
        }
    }

    public void buttonBackToMenu()
    {
        gameManager.instance.audio.play();
        if (activeLevel.bestMoves == 0 || activeLevel.bestMoves > gameManager.instance.moves) activeLevel.bestMoves = gameManager.instance.moves;
        if (activeLevel.bestTime == 0 || activeLevel.bestTime > gameManager.instance.timer) activeLevel.bestTime = gameManager.instance.timer;
        MainMenuManager.mainMenu.setActive(true);
        gameManager.instance.arena.setActive(false);
        gameManager.instance.arena.resetAreaValues();
        gameManager.instance.numpad.hide();
        setActive(false);
    }
}
