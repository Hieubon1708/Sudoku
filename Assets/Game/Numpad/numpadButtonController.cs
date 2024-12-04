using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class numpadButtonController : MonoBehaviour 
{
    public numpadController.buttonType type;
    public int buttonValue;
    public Image image;

    public enum numpadType { NUMPAD_GAME, NUMPAD_ADDLEVEL };
    public numpadType numType;
    
	void Start () 
    {
        if (type == numpadController.buttonType.BUTTON_NUMBER && buttonValue == 0) Debug.LogError("Numpad Button: " + gameObject.name + " - Bad buttonValue");

        if(Settings.invertedNumpad)
        {
            switch(buttonValue)
            {
                case 1:
                    buttonValue = 7;
                    gameObject.transform.GetComponentInChildren<Text>().text = "7";
                    break;
                case 2:
                    buttonValue = 8;
                    gameObject.transform.GetComponentInChildren<Text>().text = "8";
                    break;
                case 3:
                    buttonValue = 9;
                    gameObject.transform.GetComponentInChildren<Text>().text = "9";
                    break;
                case 7:
                    buttonValue = 1;
                    gameObject.transform.GetComponentInChildren<Text>().text = "1";
                    break;
                case 8:
                    buttonValue = 2;
                    gameObject.transform.GetComponentInChildren<Text>().text = "2";
                    break;
                case 9:
                    buttonValue = 3;
                    gameObject.transform.GetComponentInChildren<Text>().text = "3";
                    break;
            }
        }
	}

    void OnMouseDown()
    {
        if (image.color.a < 1f) return;
        gameManager.instance.audio.play();
        if (numType == numpadType.NUMPAD_GAME)
        {
            switch (type)
            {
                case numpadController.buttonType.BUTTON_NUMBER:
                    Debug.Log("Numpad Button: Set value: " + buttonValue);
                    gameManager.instance.numpad.selectedArea.setValue(buttonValue);
                    gameManager.instance.numpad.ColorChange(new Vector4(1, 1, 1, 0.5f));
                    gameManager.instance.moves++;
                    break;

                case numpadController.buttonType.BUTTON_CLEAR:
                    Debug.Log("Numpad Button: Clear value");
                    gameManager.instance.numpad.selectedArea.setValue(0);
                    gameManager.instance.moves++;
                    break;

                case numpadController.buttonType.BUTTON_BACK:
                    Debug.Log("Numpad Button: Back button");
                    break;
            }
        }
    }
}
