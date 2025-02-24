using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        playButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(ExitGame);
        instructionsButton.onClick.AddListener(OpenInstructions);
        instructionsExitButton.onClick.AddListener(CloseInstructions);
        instructionScreenElement.SetActive(false);
        UpdateHighScore();
    }

    private void StartGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    private void OpenInstructions()
    {
        instructionScreenElement.SetActive(true);
    }

    private void CloseInstructions()
    {
        instructionScreenElement.SetActive(false);
    }

    private void UpdateHighScore()
    {
        string highscoreString = "HIGHSCORE:\n" + PlayerPrefs.GetInt("Highscore");
        highscoreText.text = highscoreString;
    }
}
