using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// Class for giving the correct behaviour to the main menu in the game
/// </summary>
public class UI_MainMenu : MonoBehaviour
{
    private static UI_MainMenu instance = null;

    public static UI_MainMenu Singleton
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        /*
        if (instance == null || instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        */
        instance = this;
    }

    [SerializeField]
    private Button playButton = null;

    [SerializeField]
    private Button exitButton = null;

    [SerializeField]
    private Button instructionsButton = null;

    [SerializeField]
    private Button instructionsExitButton = null;

    [SerializeField]
    private GameObject instructionScreenElement = null;

    [SerializeField]
    private Text highscoreText = null;

    private void Start()
    {
        //Adding on click listeners to buttons to give the buttons behaviours
        playButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(ExitGame);
        instructionsButton.onClick.AddListener(OpenInstructions);
        instructionsExitButton.onClick.AddListener(CloseInstructions);
        instructionScreenElement.SetActive(false);
        UpdateHighScore();
    }

    /// <summary>
    /// Function to start the game by changing the scene
    /// </summary>
    private void StartGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    /// <summary>
    /// Function to exit the game by quitting the application
    /// </summary>
    private void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Function to open the instruction screen of the game 
    /// </summary>
    private void OpenInstructions()
    {
        instructionScreenElement.SetActive(true);
    }

    /// <summary>
    /// Function to close the instruction screen of the game
    /// </summary>
    private void CloseInstructions()
    {
        instructionScreenElement.SetActive(false);
    }

    /// <summary>
    /// Function to update the highscore variable displayed on the main menu
    /// </summary>
    private void UpdateHighScore()
    {
        string highscoreString = "HIGHSCORE:\n" + PlayerPrefs.GetInt("Highscore");
        highscoreText.text = highscoreString;
    }
}
