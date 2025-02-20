using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    public Transform enemyShootingPoint;
    public struct EnemyData
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

        public void UpdateCoords(float x, float y)
        {
            X = x;
            Y = y;
        }
        public int health { get; set; }
        public int maxHealth { get; set; }
        public int damage { get; set; }
        public float X;
        public float Y;
        public bool startShooting;

    }
    public struct EnemyTypes
    {
        public EnemyTypes(string enemyType, GameObject prefab, EnemyData enemyData)
        {
            this.enemyType = enemyType;
            this.data = enemyData;
            this.enemyPrefab = prefab;
            if (enemyType == "SHOOTING")
            {
                //SHOOTING ENEMY CHARACTERISTICS
                data.InitializeHealth(2);
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
                data.InitializeHealth(1);
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
        public EnemyData data { get; }
        public string enemyType { get; }
        public GameObject enemyPrefab { get; set; }
    }

    
}
