using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private static MainMenuManager instance = null;

    public static MainMenuManager Singleton
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
}
