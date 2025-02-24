using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float timeLeftBeforeDespawn = 5f;
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

    private void Update()
    {
        if (UI_Navigator.Singleton.GetCurrentTab() != UI_Tabs.GAME) return;
        
        timeLeftBeforeDespawn -= Time.deltaTime;
        if(timeLeftBeforeDespawn < 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision Entered");
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().DamageTaken(CameraController.Singleton.GetPlayerDamage());
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag.Equals("Boss"))
        {
            collision.gameObject.GetComponent<Enemy>().BossDamageTaken(CameraController.Singleton.GetPlayerDamage());
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag.Equals("Player"))
        {
            GameManager.Singleton.PlayerTookDamage();
            Destroy(gameObject);
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
