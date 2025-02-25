using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class SpawningManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> bullets = new List<GameObject>();

    [SerializeField]
    private List<GameObject> enemies = new List<GameObject>();

    [SerializeField]
    private Transform playerShootingPoint = null;

    [SerializeField]
    private Transform enemySpawnPoint = null;

    [SerializeField]
    private GameObject player = null;

    [SerializeField]
    private float bulletSpeed = 0f;

    [SerializeField]
    private float enemySpeed = 0f;

    [SerializeField]
    private List<Color> enemyColorVariants = new List<Color>();

    private float time = 0f;
    private float enemySpawningCooldown = 2f;
    private int enemiesSpawnedSinceStart = 0;
    private int totalEnemiesSpawned = 1;
    private bool bossSpawned = false;
    private float randomSpawnValue = 0f;
    private float randomSpawnMax = 2f;
    private float randomSpawnMin = -2f;
    private GameObject spawnedBoss = null;
    private int bossHealth = 0;
    private int enemiesNeededForNextBoss = 10;

    

    public class Enemies
    {
        public List<Enemy.EnemyTypes> enemyList = new List<Enemy.EnemyTypes>();
        
    }
    public Enemies listOfEnemies;
    
   
    private static SpawningManager instance = null;

    public static SpawningManager Singleton
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

    private void Start()
    {
        listOfEnemies = new Enemies();
        GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().MoveRotation(90);
    }
    private void Update()
    {
        if (UI_Navigator.Singleton.GetCurrentTab() == UI_Tabs.GAME)
        {
            if (bossSpawned)
            {
                var boss = GameObject.FindGameObjectWithTag("Boss") as GameObject;
                if(boss.transform.position.x < 2.6f)
                {
                    boss.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                    boss.GetComponent<Enemy>().enemyType.UpdateShootingAbility();
                }
            }
            else
            {
                time += Time.deltaTime;
                if (time >= enemySpawningCooldown)
                {

                    InstantiateEnemy();
                    time = 0f;
                    enemiesSpawnedSinceStart++;
                    totalEnemiesSpawned++;
                }
                else if (GameManager.Singleton.GetEnemiesKilledByPlayer() % enemiesNeededForNextBoss == 0 && GameManager.Singleton.GetEnemiesKilledByPlayer() != 0)
                {
                    DestroyAllEnemies();
                    InstantiateBoss();
                    bossSpawned = true;
                }
            }
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (enemy.GetComponent<Enemy>().enemyType.enemyType == "SHOOTING")
                {
                    if (enemy.transform.position.x < 2.1f)
                    {
                        enemy.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                        enemy.GetComponent<Enemy>().enemyType.UpdateShootingAbility();

                    }
                }

            }
        }
        
    }
    public void InstantiateBullet()
    {
        
        var obj = Instantiate(bullets[0], playerShootingPoint.position, playerShootingPoint.rotation) as GameObject;
        obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(bulletSpeed,0f ),ForceMode2D.Impulse);
        var parent = GameObject.FindGameObjectWithTag("BulletHolder");
        obj.transform.SetParent(parent.transform);
    }
    public void InstantiateEnemyBullet(Transform shootingPoint)
    {
        var obj = Instantiate(bullets[1], shootingPoint.position, shootingPoint.rotation) as GameObject;
        obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(-bulletSpeed, 0f), ForceMode2D.Impulse);
        obj.GetComponent<Rigidbody2D>().velocity = (GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().position - obj.GetComponentInParent<Rigidbody2D>().position).normalized;
        var parent = GameObject.FindGameObjectWithTag("BulletHolder");
        obj.transform.SetParent(parent.transform);
    }


    public void InstantiateEnemy()
    {
        //Choosing Enemy To Spawn
        int coinFlip = Random.Range(0, 2);
        int diceRoll3Sided = Random.Range(0, 2);
        int diceRoll6Sided = Random.Range(3, 8);
        var enemyChosen = coinFlip == 1 ? enemies[diceRoll3Sided] as GameObject : enemies[diceRoll6Sided] as GameObject;
        randomSpawnValue = Random.Range(randomSpawnMin, randomSpawnMax);
        int enemyVariantCount = enemyColorVariants.Count;
        Color selectedVariant = enemyColorVariants[Random.Range(0, enemyVariantCount)];
        var obj = Instantiate(enemyChosen, enemySpawnPoint.position + new Vector3(5, randomSpawnValue), Quaternion.identity);
        obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 0), ForceMode2D.Impulse);
        obj.GetComponent<SpriteRenderer>().material.color = selectedVariant;
        if (coinFlip == 1)
        {
            Enemy.EnemyTypes enemy = new Enemy.EnemyTypes("SHOOTING", obj, new Enemy.EnemyData());
            listOfEnemies.enemyList.Add(enemy);
            obj.GetComponent<Enemy>().AssignEnemyTypeData(enemy);
            obj.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            if(diceRoll6Sided < 6)
            {
                Enemy.EnemyTypes enemy = new Enemy.EnemyTypes("HAZARDS", obj, new Enemy.EnemyData());
                listOfEnemies.enemyList.Add(enemy);
                obj.GetComponent<Enemy>().AssignEnemyTypeData(enemy);
            }
            else if (diceRoll6Sided < 9)
            {
                Enemy.EnemyTypes enemy = new Enemy.EnemyTypes("DEBRIES", obj, new Enemy.EnemyData());
                listOfEnemies.enemyList.Add(enemy);
                obj.GetComponent<Enemy>().AssignEnemyTypeData(enemy);
            }
        }
        
        
        
        
        var parent = GameObject.FindGameObjectWithTag("EnemyHolder");
        obj.transform.SetParent(parent.transform);
    }

    public void InstantiateBoss()
    {
        var bossChosen = enemies[Random.Range(9, 11)] as GameObject;
        var obj = Instantiate(bossChosen, enemySpawnPoint.position,enemySpawnPoint.rotation);
        obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 0), ForceMode2D.Impulse);
        Enemy.EnemyTypes enemy = new Enemy.EnemyTypes("BOSS", obj, new Enemy.EnemyData());
        listOfEnemies.enemyList.Add(enemy);
        obj.GetComponent<Enemy>().AssignEnemyTypeData(enemy);
        obj.transform.rotation = Quaternion.Euler(0, 0, 90);

        bossHealth = enemy.data.health;
        var parent = GameObject.FindGameObjectWithTag("EnemyHolder");
        obj.transform.SetParent(parent.transform);
        spawnedBoss = obj;

        enemySpawningCooldown -= (enemySpawningCooldown / 10) * 2;
        enemiesNeededForNextBoss *= 2;
    }

    public void RemoveEnemyFromList(Enemy.EnemyTypes enemyType)
    {
        listOfEnemies.enemyList.Remove(enemyType);
    }

    public void BossDied()
    {
        bossSpawned = false;
    }

    public void DestroyAllEnemies()
    {
        List<GameObject> enemiesToDestroy = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        foreach(GameObject enemy in enemiesToDestroy)
        {
            enemy.GetComponent<Enemy>().DeconstructEnemy();
        }
    }
}
