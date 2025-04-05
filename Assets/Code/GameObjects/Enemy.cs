using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

/// <summary>
/// Public class of enemy to be placed onto enemies spawned inside of the game.
/// </summary>
public class Enemy : MonoBehaviour
{
    [SerializeField]
    public Transform enemyShootingPoint;
    private float timeSinceLastShot = 0f;
    private float timeToDespawn = 10f;

    /// <summary>
    /// EnemyData public class to store information on enemies
    /// </summary>
    public class EnemyData
    {
        public void InitializeHealth(int maxHealth)
        {
            this.maxHealth = maxHealth;
            health = Random.Range(1, maxHealth);
        }
        public void InitializeDamage(int dmgMax)
        {
            damage = Random.Range(1, dmgMax);
        }

        public void InitializeEnemyType()
        {
            startShooting = false;
        }

        public void AllowedToStartFireing(bool result)
        {
            startShooting = result;
        }

        public int health { get; set; }
        public int maxHealth { get; set; }
        public int damage { get; set; }
        public bool startShooting;

    }

    /// <summary>
    /// Public class used to store vitial information about enemies and dictate their behaviour depending on what type of enemy they are
    /// </summary>
    public class EnemyTypes
    {
        public EnemyTypes(string enemyType, GameObject prefab, EnemyData enemyData)
        {
            this.enemyType = enemyType;
            this.data = enemyData;
            this.enemyPrefab = prefab;
            if (enemyType == "SHOOTING")
            {
                //SHOOTING ENEMY CHARACTERISTICS
                data.InitializeHealth(3);
                data.InitializeDamage(1);
                data.InitializeEnemyType();
            }
            else if (enemyType == "DEBRIES")
            {
                data.InitializeHealth(3);
                data.InitializeDamage(2);
                data.InitializeEnemyType();
            }
            else if (enemyType == "HAZARDS")
            {
                data.InitializeHealth(4);
                data.InitializeDamage(10);
                data.InitializeEnemyType();
            }
            else if (enemyType == "BOSS")
            {
                data.InitializeHealth(Random.Range(20, 50));
                data.InitializeDamage(1);
                data.InitializeEnemyType();
            }
        }

        public void UpdateShootingAbility()
        {
            data.AllowedToStartFireing(true);
        }


        public EnemyData data { get; set; }
        public string enemyType { get; set; }
        public GameObject enemyPrefab { get; set; }
    }
    public EnemyTypes enemyType;
    [SerializeField]
    private int health;

    private void Update()
    {
        if (UI_Navigator.Singleton.GetCurrentTab() != UI_Tabs.GAME) return;
        if (enemyType == null) return;
        if(enemyType.enemyType == "HAZARDS" || enemyType.enemyType == "DEBRIES")
        {
            timeToDespawn -= Time.deltaTime;
            transform.Rotate(new Vector3(0, 0, Random.Range(1, 20)) * Time.deltaTime * 20);
        }
        if (!enemyType.data.startShooting) return;
        if(enemyShootingPoint == null) return;
        timeSinceLastShot += Time.deltaTime;
        if (enemyType.enemyType == "BOSS")
        {
            if(timeSinceLastShot > 1f)
            {
                SpawningManager.Singleton.InstantiateEnemyBullet(enemyShootingPoint);
                timeSinceLastShot = 0f;
            }
        }
        else if (timeSinceLastShot > 2f) 
        {
            SpawningManager.Singleton.InstantiateEnemyBullet(enemyShootingPoint);
            timeSinceLastShot = 0f;
        }
        if (timeToDespawn <= 0f)
        {
            DeconstructEnemy();
        }
        
    }
    
    /// <summary>
    /// Public function to assign enemy type data to the current enemy from the spawning script
    /// </summary>
    /// <param name="enemyType"></param>
    public void AssignEnemyTypeData(EnemyTypes enemyType)
    {
        this.enemyType = enemyType;
        health = enemyType.data.health;
    }

    /// <summary>
    /// Function to deconstruct enemies by first removing them from saved list of enemies and then destroying its gameobject.
    /// </summary>
    public void DeconstructEnemy()
    {
        ///Checking enemy type
        if(enemyType.enemyType == "SHOOTING")
        {
            if (PlayerPrefs.HasKey("EnemysKilled"))
            {
                PlayerPrefs.SetInt("EnemysKilled", PlayerPrefs.GetInt("EnemysKilled") + 1);
            }
            else
            {
                PlayerPrefs.SetInt("EnemysKilled", 1);
            }
            if(PlayerPrefs.GetInt("EnemysKilled") >= 25)
            {
                Social.ReportProgress(GPGSIds.achievement_kill_25_enemies, 100.0, success => Debug.Log(success ? "Achievement unlocked!" : "Failed to unlock achievement"));
            }
            else if (PlayerPrefs.GetInt("EnemysKilled") >= 5)
            {
                Social.ReportProgress(GPGSIds.achievement_kill_5_enemies, 100.0, success => Debug.Log(success ? "Achievement unlocked!" : "Failed to unlock achievement"));
            }

        }
        else if(enemyType.enemyType == "DEBRIES" || enemyType.enemyType == "HAZARDS")
        {
            if (PlayerPrefs.HasKey("DebrisDestroyed"))
            {
                PlayerPrefs.SetInt("DebrisDestroyed", PlayerPrefs.GetInt("DebrisDestroyed") + 1);
            }
            else
            {
                PlayerPrefs.SetInt("DebrisDestroyed", 1);
            }
            if (PlayerPrefs.GetInt("EnemysKilled") >= 25)
            {
                Social.ReportProgress(GPGSIds.achievement_destroy_25_space_debris, 100.0, success => Debug.Log(success ? "Achievement unlocked!" : "Failed to unlock achievement"));
            }
            else if (PlayerPrefs.GetInt("EnemysKilled") >= 5)
            {
                Social.ReportProgress(GPGSIds.achievement_destroy_5_space_derbies,100.0, success => Debug.Log(success ? "Achievement unlocked!" : "Failed to unlock achievement"));
            }

        }
        PlayerPrefs.Save();
        SpawningManager.Singleton.RemoveEnemyFromList(enemyType);
        Destroy(gameObject);
    }

    /// <summary>
    /// Function to deconstruct bosses it works by first removing them from saved list of enemies and then destroying its gameobject.
    /// </summary>
    public void DeconstructBoss()
    {
        SpawningManager.Singleton.RemoveEnemyFromList(enemyType);
        Destroy(gameObject);

    }

    /// <summary>
    /// Function for ehaviour regarding taking damage
    /// </summary>
    /// <param name="currentPlayerDamage"></param>
    public void DamageTaken(int currentPlayerDamage)
    {
        health -= currentPlayerDamage;
        if(health <= 0)
        {
            DeconstructEnemy();
            GameManager.Singleton.EnemyKilled();
        }
        GameManager.Singleton.EnemyHurt();
    }

    /// <summary>
    /// Function for behaviour of a boss taking damange
    /// </summary>
    /// <param name="currentPlayerDamage"></param>
    public void BossDamageTaken(int currentPlayerDamage)
    {
        health -= currentPlayerDamage;
        if(health <=0)
        {
            DeconstructBoss();
            GameManager.Singleton.BossDied();
            GameManager.Singleton.EnemyKilled();
        }
        GameManager.Singleton.EnemyHurt();
    }

    /// <summary>
    /// Collisiion behaviour adding for enemies
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player") && (enemyType.enemyType.Equals("HAZARDS") || enemyType.enemyType.Equals("DEBRIES"))) 
        {
            GameManager.Singleton.PlayerTookDamage();
            DeconstructEnemy();
        }
    }

}
