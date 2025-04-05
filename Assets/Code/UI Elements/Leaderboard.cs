using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    private float timer;
    private float timeCooldown = 2f;
    [SerializeField]
    private Button LeaderBoardButton = null;
    private string mStatus;
    private static Leaderboard instance = null;

    public static Leaderboard Singleton
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
        }
    }
    private void Start()
    {
        LeaderBoardButton.onClick.AddListener(OpenLeaderboard);
    }

    private void UpdateLeaderboardWithNewHighScore(long score)
    {
        
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (timer > timeCooldown)
        {
            if (PlayerPrefs.HasKey("Highscore"))
            {
                long highscore = (long)PlayerPrefs.GetInt("Highscore");
                UpdateLeaderboardWithNewHighScore(highscore);
            }
            timer = 0f;
        }
    }

    private void OpenLeaderboard()
    {

        Social.ShowLeaderboardUI();
        /*
        ILeaderboard lb = PlayGamesPlatform.Instance.CreateLeaderboard();
        lb.userScope = UserScope.FriendsOnly;
        lb.id = "CgkIy9Do4aEQEAIQCw";
        lb.LoadScores(ok =>
        {
            if (ok)
            {
                LoadUsersAndDisplay(lb);
            }
            else
            {
                mStatus = "Leaderboard loading: " + lb.title + " ok = " + ok;
            }
        });
        */

    }

    internal void LoadUsersAndDisplay(ILeaderboard lb)
    {
        // get the use ids
        List<string> userIds = new List<string>();

        foreach (IScore score in lb.scores)
        {
            userIds.Add(score.userID);
        }

        Social.LoadUsers(userIds.ToArray(), (users) =>
        {
            mStatus = "Leaderboard loading: " + lb.title + " count = " +
                      lb.scores.Length;
            foreach (IScore score in lb.scores)
            {
                IUserProfile user = FindUser(users, score.userID);
                mStatus += "\n" + score.formattedValue + " by " +
                           (string)(
                               (user != null) ? user.userName : "**unk_" + score.userID + "**");
            }
        });
    }

    private IUserProfile FindUser(IUserProfile[] users, string userid)
    {
        foreach (IUserProfile user in users)
        {
            if (user.id == userid)
            {
                return user;
            }
        }

        return null;
    }

}

