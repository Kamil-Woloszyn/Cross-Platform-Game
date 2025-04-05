using GameAnalyticsSDK;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;
using System;
using System.Linq;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEditor;

public class ServiceManager : MonoBehaviour , IGameAnalyticsATTListener
{
    [SerializeField]
    private Button googlePlayGamesLoginButton = null;
    private static ServiceManager instance = null;

    public static ServiceManager Singleton
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }
    //Private Flags
    private bool flagGameStart = false;
    private bool flagCollectData = false;

    private void Start()
    {
        /*****************
         * GAME ANALYTICS
         *****************/
        GameAnalytics.Initialize();

        string region = Application.systemLanguage.ToString();
        GameAnalytics.SetCustomDimension01(region);
        Debug.Log("Region set to: " + region);

        /******************
         * GOOGLE PLAY GAMES
         * ****************/
        /*
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(success => { Debug.Log("Signed in to Google Play || Google Play sign-in failed"); });
        */
        googlePlayGamesLoginButton.onClick.AddListener(GooglePlaySignInManual);
        Social.localUser.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            Debug.Log("Sign in Success");
            // Disable Signin button in the game to manually sign in as the user is already signed in
            googlePlayGamesLoginButton.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Sign in Fail");
            // Enable Signin button in the game to manually sign in
            googlePlayGamesLoginButton.gameObject.SetActive(true);
        }
    }

    internal void ProcessAuthentication(bool status)
    {
        if (status)
        {
            Debug.Log("Sign in Success!");
            // Continue with Play Games Services
            googlePlayGamesLoginButton.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Sign in Fail!");
            // Exit Game
            googlePlayGamesLoginButton.gameObject.SetActive(true);
        }
    }

    public void GameAnalyticsATTListenerNotDetermined() { }
    public void GameAnalyticsATTListenerRestricted() { }
    public void GameAnalyticsATTListenerDenied() { }
    public void GameAnalyticsATTListenerAuthorized() { }

    
    private void GooglePlaySignInManual()
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
    }
    public void SetFlagGameStarted(bool flag)
    {
        flagGameStart = flag;
        CheckCollectionFlagStatuses();
    }

    public void SetFlagCollectData(bool flag)
    {
        flagCollectData = flag;
        CheckCollectionFlagStatuses();
    }

    private void CheckCollectionFlagStatuses()
    {
        if (flagCollectData && flagGameStart)
        {
            Debug.Log("Data Collected.");
            SetFlagGameStarted(!flagGameStart);
            SetFlagCollectData(!flagCollectData);
        }
    }
    public void CollectDataPlayerKillConfirmed()
    {
        GameAnalytics.NewDesignEvent("player:killedEnemy");
    }

    public void UpdateLeaderboardHighScore(int score)
    {

    }


}
