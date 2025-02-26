using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Class for navigating through the different ingame ui elements
/// </summary>
public class UI_Navigator : MonoBehaviour
{
    //UI ELEMENTS
    [SerializeField]
    private GameObject gameElements = null;

    [SerializeField]
    private GameObject shopElements = null;

    [SerializeField]
    private GameObject pausedElements = null;

    [SerializeField]
    private GameObject gameOverElements = null;

    //BUTTONS UI
    [SerializeField]
    private Button exitToMainMenuButton = null;

    [SerializeField]
    private Button exitShopButton = null;

    [SerializeField]
    private Button exitPauseButton = null;

    //current tab opened in the game as a global variable
    [SerializeField]
    UI_Tabs currentTab;

    //Instance of the Singleton class of this class
    private static UI_Navigator instance = null;

    public static UI_Navigator Singleton
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null || instance != this)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        currentTab = UI_Tabs.GAME;
        exitToMainMenuButton.onClick.AddListener(GoToMainMenu);
        //exitShopButton.onClick.AddListener(ResumeGame);
        exitPauseButton.onClick.AddListener(ResumeGame);
    }

    /// <summary>
    /// Class which lets UI to be switched to a different ui element inside the game
    /// </summary>
    /// <param name="tab"></param>
    public void UI_GOTO(UI_Tabs tab)
    {
        CloseAllUI();
        currentTab = tab;
        if (tab == UI_Tabs.MAIN_MENU)
        {
            SceneManager.LoadScene("MainMenu");

        }
        else if(tab == UI_Tabs.SHOP)
        {
            shopElements.SetActive(true);
        }
        else if(tab == UI_Tabs.GAME)
        {
            gameElements.SetActive(true);
        }
        else if(tab ==UI_Tabs.GAME_OVER)
        {
            gameOverElements.SetActive(true);
        }
        else if(tab == UI_Tabs.GAME_PAUSED)
        {
            pausedElements.SetActive(true);
        }
    }

    /// <summary>
    /// Function to close all of the UI elements
    /// </summary>
    public void CloseAllUI()
    {
        shopElements.SetActive(false);
        gameElements.SetActive(false);
        gameOverElements.SetActive(false);
        pausedElements.SetActive(false);


    }

    /// <summary>
    /// Function for going to main menu ui to use for buttons
    /// </summary>
    public void GoToMainMenu()
    {
        UI_GOTO(UI_Tabs.MAIN_MENU);
    }

    /// <summary>
    /// Function for setting the games state to go back to the game state with the click/tap of a button
    /// </summary>
    public void ResumeGame()
    {
        UI_GOTO(UI_Tabs.GAME);
    }

    /// <summary>
    /// Function to access the current tab avaible inside of this class as a global varaible
    /// </summary>
    /// <returns></returns>
    public UI_Tabs GetCurrentTab()
    {
        return currentTab;
    }

}
