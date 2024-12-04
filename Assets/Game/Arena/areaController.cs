using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class areaController : MonoBehaviour
{
    public int value;
    public bool canEdit = true;
    public Image image;

    public enum arenaType { ARENA_GAME, ARENA_ADDLEVEL };
    public arenaType areaType;

    private TextMeshProUGUI valueText;

    public void Init()
    {
        valueText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        valueText.text = " ";

        value = 0;
    }

    void OnMouseDown()
    {
        gameManager.instance.audio.play();

        if (areaType == arenaType.ARENA_GAME)
        {
            if (!gameManager.instance.numpad.isDisplayed() && canEdit)
            {
                Debug.Log(gameObject.transform.parent.transform.parent.name + " - " + gameObject.name);
                gameManager.instance.ColorSet(image);
                gameManager.instance.numpad.setSelectedArea(gameObject.GetComponent<areaController>());
                gameManager.instance.numpad.ColorChange(new Vector4(0, 124f / 255f, 123f / 255f, 1f));
            }
        }
    }

    public void reset()
    {
        value = 0;
        canEdit = true;
        valueText.text = " ";
        gameObject.GetComponent<Image>().color = Color.white;
    }

    public void setValue(int val)
    {
        value = val;

        if (val == 0) valueText.text = " ";
        else valueText.text = val.ToString();
    }

    public void setConstValue(int val)
    {
        value = val;
        canEdit = false;
        valueText.text = val.ToString();
    }
}
