using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
/// <summary>
/// Class of GameManager which overlooks the mechanisms of the game 
/// </summary>
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
        //Checking if the game ui is currently in a specific screen, and incase the user is in that ui then check if a keyboard key is pressed
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

        //Updating player hud text field
        UpdatePlayerHud();
        //Updating the variables for highscores achieved by the player
        UpdateHighScore();
    }

    /// <summary>
    /// Function for behaviour of dictating of what happens when the boss dies inside of the game
    /// </summary>
    public void BossDied()
    {
        SpawningManager.Singleton.BossDied();
        currentLevel += 1;
    }

    /// <summary>
    /// Function for behaviour of what happens when the player dies. It goes to the game over screen.
    /// </summary>
    public void PlayerDied()
    {
        ServiceManager.Singleton.SetFlagCollectData(true);
        UI_Navigator.Singleton.UI_GOTO(UI_Tabs.GAME_OVER);
    }

    /// <summary>
    /// Function which dictates what happens when the player recieves damage done by anything.
    /// </summary>
    public void PlayerTookDamage()
    {
        playerHealth -= 1;
        //Lowering total score as a punishment for taking damage
        if(playerHealth <= 0)
        {
            PlayerDied();
        }
    }   


    /// <summary>
    /// Function which adds a set amount of score to the total score inside of the game score quantity.
    /// </summary>
    /// <param name="score"></param>
    public void AddScore(int score)
    {
        currentScore += score;
    }

    /// <summary>
    /// Function which updates the variables inside of the text hud.
    /// </summary>
    public void UpdatePlayerHud()
    {
        if (UI_Navigator.Singleton.GetCurrentTab() == UI_Tabs.GAME)
        {
            HUD_Text.text = "HEALTH: " + playerHealth + "\nLEVEL: " + currentLevel + "\nSCORE: " + currentScore + "\nENEMIES KILLED: " + enemiesKilled;

        }
    }

    /// <summary>
    /// Getter for variable 'enemiesKilled'
    /// </summary>
    /// <returns></returns>
    public int GetEnemiesKilledByPlayer()
    {
        return enemiesKilled;
    }

    /// <summary>
    /// Function which adds score if an enemy is hurt 
    /// </summary>
    public void EnemyHurt()
    {
        AddScore(Random.Range(400,500));
    }

    /// <summary>
    /// Function which updates high score variable for the player currently playing
    /// </summary>
    public void UpdateHighScore()
    {
        if (PlayerPrefs.GetInt("Highscore") < currentScore)
        {
            PlayerPrefs.SetInt("Highscore", currentScore);
            PlayerPrefs.Save();
        }


        if (PlayerPrefs.HasKey("Highscore"))
        {
            PlayerPrefs.SetInt("Highscore", PlayerPrefs.GetInt("Highscore") + 1);
        }
        else
        {
            PlayerPrefs.SetInt("Highscore", 1);
        }
        if(PlayerPrefs.GetInt("DebrisDestroyed") > 5000000)
        {
            Social.ReportProgress(GPGSIds.achievement_ascended_toward_radiant_astral_realms,100.0, success => Debug.Log(success ? "Achievement unlocked!" : "Failed to unlock achievement"));
        }
        else if(PlayerPrefs.GetInt("DebrisDestroyed") > 1000000)
        {
            Social.ReportProgress(GPGSIds.achievement_legend, 100.0, success => Debug.Log(success ? "Achievement unlocked!" : "Failed to unlock achievement"));
        }
        else if (PlayerPrefs.GetInt("DebrisDestroyed") > 250000)
        {
            Social.ReportProgress(GPGSIds.achievement_champion,100.0, success => Debug.Log(success ? "Achievement unlocked!" : "Failed to unlock achievement"));
        }
        else if (PlayerPrefs.GetInt("DebrisDestroyed") > 50000)
        {
            Social.ReportProgress(GPGSIds.achievement_strategist,100.0, success => Debug.Log(success ? "Achievement unlocked!" : "Failed to unlock achievement"));
        }
        else if (PlayerPrefs.GetInt("DebrisDestroyed") > 10000)
        {
            Social.ReportProgress(GPGSIds.achievement_rookie,100.0, success => Debug.Log(success ? "Achievement unlocked!" : "Failed to unlock achievement"));
        }
        PlayerPrefs.Save();

    }

    /// <summary>
    /// Function which increases variable 'enemiesKilled'
    /// </summary>
    public void EnemyKilled()
    {
        enemiesKilled += 1;
    }
}
