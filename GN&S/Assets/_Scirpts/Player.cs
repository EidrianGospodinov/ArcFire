using System;
using System.Collections;
using System.Collections.Generic;
using OpenCover.Framework.Model;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    //serialized
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPosition;
    [SerializeField] float fireRate = 0.5f;
    [SerializeField]float bulletSpeed = 1f;
    [SerializeField] private AudioClip playerShotSound;
    [SerializeField] GameObject bulletFiringEffect;
    
    //public
    
    //private
    private Rigidbody rb; 
    private float nextFire = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        if(Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (Time.time >= nextFire)
        {
            nextFire = Time.time + fireRate;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition.position, Quaternion.identity);
            var bulletDir = transform.rotation * Vector3.forward;
            bullet.GetComponent<BulletController>().InitializeBullet(bulletDir, bulletSpeed, 10);
            AudioManager.instance.Play3D(playerShotSound, transform.position);
            
            VFXManager.Instance.PlayVFX(bulletFiringEffect, bulletSpawnPosition.position, bulletDir);
        }
    }

    private void Move()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            return;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        var rotation = Quaternion.LookRotation(new Vector3(x, 0, z));
        rb.rotation = rotation;
        var movement = transform.forward * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
    }
}