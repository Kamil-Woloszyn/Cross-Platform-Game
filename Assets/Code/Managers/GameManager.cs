using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Text HUD_Text = null;
    //Variables
    private int playerHealth = 3;
    private int currentLevel = 1;
    private int currentScore = 0;
    private int enemiesKilled = 0;

    //Singleton Instance
    private static GameManager instance = null;

    public static GameManager Singleton
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

    private void Update()
    {
        if(UI_Navigator.Singleton.GetCurrentTab() == UI_Tabs.GAME)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                UI_Navigator.Singleton.UI_GOTO(UI_Tabs.GAME_PAUSED);
            }
            if(playerHealth <= 0)
            {
                PlayerDied();
            }
        }

        if(UI_Navigator.Singleton.GetCurrentTab() == UI_Tabs.GAME_PAUSED)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UI_Navigator.Singleton.UI_GOTO(UI_Tabs.GAME);
            }
        }

        if (UI_Navigator.Singleton.GetCurrentTab() == UI_Tabs.MAIN_MENU)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        UpdatePlayerHud();
        UpdateHighScore();
    }
    public void BossDied()
    {
        SpawningManager.Singleton.BossDied();
        currentLevel += 1;
    }

    public void PlayerDied()
    {
        UI_Navigator.Singleton.UI_GOTO(UI_Tabs.GAME_OVER);
    }

    public void PlayerTookDamage()
    {
        playerHealth -= 1;
        //Lowering total score as a punishment for taking damage
        currentScore /= 2;

        if(playerHealth <= 0)
        {
            PlayerDied();
        }
    }   

    public void AddScore(int score)
    {
        currentScore += score;
    }

    public void UpdatePlayerHud()
    {
        if (UI_Navigator.Singleton.GetCurrentTab() == UI_Tabs.GAME)
        {
            HUD_Text.text = "HEALTH: " + playerHealth + "\nLEVEL: " + currentLevel + "\nSCORE: " + currentScore + "\nENEMIES KILLED: " + enemiesKilled;

        }
    }

    public int GetEnemiesKilledByPlayer()
    {
        return enemiesKilled;
    }

    public void EnemyHurt()
    {
        AddScore(Random.Range(400,500));
    }

    public void UpdateHighScore()
    {
        if (PlayerPrefs.GetInt("Highscore") < currentScore)
        {
            PlayerPrefs.SetInt("Highscore", currentScore);
            PlayerPrefs.Save();
        }

    }

    public void EnemyKilled()
    {
        enemiesKilled += 1;
    }
}
