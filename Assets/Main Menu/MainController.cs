using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour
{
    public bool active;

    public void setActive(bool state)
    {
        switch (state)
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

    public void buttonPlay()
    {
        Debug.Log("Play game button");
        gameManager.instance.audio.play();
    }

    public void buttonResolve()
    {
        Debug.Log("Resolve sudoku button");
        gameManager.instance.audio.play();
    }

    public void buttonStatistics()
    {
        Debug.Log("Statistics button");
        gameManager.instance.audio.play();
    }

    public void buttonHowTo()
    {
        Debug.Log("How To button");
        gameManager.instance.audio.play();
    }

    public void buttonExitGame()
    {
        Debug.Log("Exit Game button");
        gameManager.instance.audio.play();
    }

    public void buttonDEBUGnewLevel()
    {
        Debug.LogWarning("[DEBUG] Add new levels button");
        gameManager.instance.audio.play();
    }

    public void buttonSettings()
    {
        Debug.Log("Settings button");
        gameManager.instance.audio.play();
    }
}
