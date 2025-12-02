using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    //serialized
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPosition;
    [SerializeField] float fireRate = 0.5f;
    [SerializeField] float bulletSpeed = 1f;
    [SerializeField] private AudioClip playerShotSound;
    [SerializeField] GameObject bulletFiringEffect;
    [SerializeField] Slider healthBar;
    [SerializeField] int bulletDamage;
    

    //public
    public int health = 100;

    public delegate void EnemyKilled();

    public static event EnemyKilled OnEnemyKilled;
    
    //private
    private Rigidbody rb;
    private float nextFire = 0f;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.LookAt(other.transform);
            Fire();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            BulletController bulletController = collision.gameObject.GetComponent<BulletController>();
            if (bulletController != null)
            {
                TakeDamage(bulletController.bulletDamage);
            }
        }
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.value = health;
        if (health <= 0)
        {
            EnemyDied();
        }
    }

    private void EnemyDied()
    {
        if (OnEnemyKilled != null)
        {
            OnEnemyKilled.Invoke();
        }
        gameObject.SetActive(false);
    }

    private void Fire()
    {
        if (Time.time >= nextFire)
        {
            nextFire = Time.time + fireRate;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition.position, Quaternion.identity);
            var bulletDir = transform.rotation * Vector3.forward;
            bullet.GetComponent<BulletController>().InitializeBullet(bulletDir, bulletSpeed, bulletDamage);
            AudioManager.instance.Play3D(playerShotSound, transform.position);

            VFXManager.Instance.PlayVFX(bulletFiringEffect, bulletSpawnPosition.position, bulletDir);
        }


    }
}
