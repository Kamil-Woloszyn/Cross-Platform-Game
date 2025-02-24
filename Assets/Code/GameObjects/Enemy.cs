using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    public Transform enemyShootingPoint;
    private float timeSinceLastShot = 0f;
    private float timeToDespawn = 10f;
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
    public void AssignEnemyTypeData(EnemyTypes enemyType)
    {
        this.enemyType = enemyType;
        health = enemyType.data.health;
    }

    public void DeconstructEnemy()
    {
        SpawningManager.Singleton.RemoveEnemyFromList(enemyType);
        Destroy(gameObject);
    }

    public void DeconstructBoss()
    {
        SpawningManager.Singleton.RemoveEnemyFromList(enemyType);
        Destroy(gameObject);

    }

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player") && (enemyType.enemyType.Equals("HAZARDS") || enemyType.enemyType.Equals("DEBRIES"))) 
        {
            GameManager.Singleton.PlayerTookDamage();
            DeconstructEnemy();
        }
    }

}
