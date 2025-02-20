using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class SpawningManager : MonoBehaviour
{
    [SerializeField]
    private GameObject bullet = null;

    [SerializeField]
    private GameObject bossEnemy = null;

    [SerializeField]
    private List<GameObject> enemies = new List<GameObject>();

    [SerializeField]
    private Transform playerShootingPoint = null;

    [SerializeField]
    private Transform enemySpawnPoint = null;

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
    private float randomSpawnMax = 3f;
    private float randomSpawnMin = -3f;
    private GameObject spawnedBoss = null;
    private int bossHealth = 0;

    

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
    }
    private void Update()
    {
        if(bossSpawned)
        {
            if(bossHealth <= 0)
            {
                bossSpawned = false;

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
            else if (enemiesSpawnedSinceStart >= 10)
            {
                enemySpawningCooldown -= (enemySpawningCooldown / 100) * 10;
                enemiesSpawnedSinceStart = 0;
                //InstantiateBoss();
            }
            else if(totalEnemiesSpawned % 50 == 0)
            {
                InstantiateBoss();
                bossSpawned = true;
            }
        }
        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if(enemy.transform.position.x < 2.1f)
            {
                Debug.Log("Updating Enemy Velocity");
                enemy.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                
                foreach (Enemy.EnemyTypes enemys in listOfEnemies.enemyList)
                {
                    if(enemy.gameObject.Equals(enemys.enemyPrefab) && enemys.enemyType.Equals("SHOOTING"))
                    {
                        Destroy(enemys.enemyPrefab);
                        //listOfEnemies.enemyList.Remove(enemys);
                    }
                }
            }
        }
        
        
    }
    public void InstantiateBullet()
    {

        var obj = Instantiate(bullet, playerShootingPoint.position, playerShootingPoint.rotation) as GameObject;
        obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(bulletSpeed,0f ),ForceMode2D.Impulse);
        var parent = GameObject.FindGameObjectWithTag("BulletHolder");
        obj.transform.SetParent(parent.transform);
    }

    public void InstantiateEnemy()
    {
        
        randomSpawnValue = Random.Range(randomSpawnMin, randomSpawnMax);
        int enemyVariantCount = enemyColorVariants.Count;
        Color selectedVariant = enemyColorVariants[Random.Range(0,enemyVariantCount)];
        Quaternion rotation = new Quaternion(0,0,125,0);
        var obj = Instantiate(enemies[0], enemySpawnPoint.position + new Vector3(5, randomSpawnValue), rotation);
        obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1,0),ForceMode2D.Impulse);
        obj.GetComponent<SpriteRenderer>().material.color = selectedVariant;
        obj.gameObject.transform.rotation = rotation;
        Enemy.EnemyTypes enemy = new Enemy.EnemyTypes("SHOOTING", obj, new Enemy.EnemyData());
        listOfEnemies.enemyList.Add(enemy);
        //var parent = GameObject.FindGameObjectWithTag("EnemyHolder");
        //obj.transform.SetParent(parent.transform);
    }

    public void InstantiateBoss()
    {
        randomSpawnValue = Random.Range(randomSpawnMin, randomSpawnMax);
        var obj = Instantiate(bossEnemy,enemySpawnPoint.position + new Vector3(0,randomSpawnValue),enemySpawnPoint.rotation);
        obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 0), ForceMode2D.Impulse);
        var parent = GameObject.FindGameObjectWithTag("EnemyHolder");
        obj.transform.SetParent(parent.transform);
        spawnedBoss = obj;
        bossHealth = 10;
    }

}
