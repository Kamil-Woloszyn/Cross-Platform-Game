using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField]
    UI_Tabs currentTab;
    private void Start()
    {
        currentTab = UI_Tabs.GAME;
        exitToMainMenuButton.onClick.AddListener(GoToMainMenu);
        //exitShopButton.onClick.AddListener(ResumeGame);
        exitPauseButton.onClick.AddListener(ResumeGame);
    }

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

    public void CloseAllUI()
    {
        shopElements.SetActive(false);
        gameElements.SetActive(false);
        gameOverElements.SetActive(false);
        pausedElements.SetActive(false);


    }

    public void GoToMainMenu()
    {
        UI_GOTO(UI_Tabs.MAIN_MENU);
    }

    public void ResumeGame()
    {
        UI_GOTO(UI_Tabs.GAME);
    }

    public UI_Tabs GetCurrentTab()
    {
        return currentTab;
    }

}
