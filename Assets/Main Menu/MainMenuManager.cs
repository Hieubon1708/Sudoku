using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour 
{
    public static MainController mainMenu;
    public static SelectModeController selectModePanel;
	
	void Start () 
    {
        mainMenu = gameObject.transform.GetComponentInChildren<MainController>();
        selectModePanel = gameObject.transform.GetComponentInChildren<SelectModeController>();

        mainMenu.setActive(true);
	}
}
