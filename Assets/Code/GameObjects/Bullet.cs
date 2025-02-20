using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public struct BulletData
    {
        public void InitializeBulletData(string type, GameObject obj)
        {
            bulletType = type;
            bulletPrefab = obj;
        }
        public void SetBulletType(string type)
        {
            bulletType = type;
        }
        public void SetBulletPrefab(GameObject obj)
        {
            bulletPrefab = obj;
        }
        public string bulletType;
        public GameObject bulletPrefab;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision Entered");
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            foreach(Enemy.EnemyTypes enemy in SpawningManager.Singleton.listOfEnemies.enemyList.ToList())
            {
                if(enemy.enemyPrefab.Equals(collision.gameObject))
                {
                    SpawningManager.Singleton.listOfEnemies.enemyList.Remove(enemy);
                    Destroy(collision.gameObject);
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                Debug.Log("bullet hit by bullet ");
            }
        }
    }
}
