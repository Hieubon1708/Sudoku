using UnityEngine;
using System.Collections;

public class SelectModeController : MonoBehaviour 
{
    public bool active;
    public GameObject gamePlay;

	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Escape)) setActive(false);
	}

    public void setActive(bool state)
    {
        switch(state)
        {
            case true:
                active = true;
                gameObject.SetActive(true);
                break;
            case false:
                active = false;
                gameObject.SetActive(false);
                break;
        }
    }

    //Buttons - Predefined
    public void buttonPreEasy()
    {
        Debug.Log("Predefined Easy button");
        gameManager.instance.audio.play();
        setActive(false);
    }

    public void buttonPreMedium()
    {
        Debug.Log("Predefined Medium button");
        gameManager.instance.audio.play();
        setActive(false);
    }

    public void buttonPreHard()
    {
        Debug.Log("Predefined Hard button");
        gameManager.instance.audio.play();
        setActive(false);
    }

    //Buttons - Random
    public void buttonRandEasy()
    {
        Debug.Log("Random Easy button");
        gameManager.instance.audio.play();
        gameManager.instance.randomGame = true;
        gameManager.instance.arena.resetAreaValues();
        gameManager.instance.arena.setAreaValues(LevelManager.easyLevels[Random.Range(0,LevelManager.easyLevels.Count-1)]);
        gamePlay.SetActive(true);
        setActive(false);
    }

    public void buttonRandMedium()
    {
        Debug.Log("Random Medium button");
        gameManager.instance.audio.play();
        gameManager.instance.randomGame = true;
        gameManager.instance.arena.resetAreaValues();
        gameManager.instance.arena.setAreaValues(LevelManager.mediumLevels[Random.Range(0, LevelManager.mediumLevels.Count - 1)]);
        gamePlay.SetActive(true);
        setActive(false);
    }

    public void buttonRandHard()
    {
        Debug.Log("Random Hard button");
        gameManager.instance.audio.play();
        gameManager.instance.randomGame = true;
        gameManager.instance.arena.resetAreaValues();
        gameManager.instance.arena.setAreaValues(LevelManager.hardLevels[Random.Range(0, LevelManager.hardLevels.Count - 1)]);
        gamePlay.SetActive(true);
        setActive(false);
    }
}
