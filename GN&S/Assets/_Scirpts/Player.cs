using System;
using System.Collections;
using System.Collections.Generic;
using OpenCover.Framework.Model;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb; 
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawn;
    [SerializeField] float fireRate = 0.5f;
    [SerializeField]float bulletSpeed = 1f;
    private float nextFire = 0f;
    // Start is called before the first frame update
    void Start()
    {
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
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
            bullet.GetComponent<BulletController>().InitializeBullet(transform.rotation * Vector3.forward, bulletSpeed);
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