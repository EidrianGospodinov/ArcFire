using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] AudioClip bulletSound;
    [SerializeField] private GameObject bulletImpactEffect;
    public int bulletDamage;
    
    private Rigidbody rb;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeBullet(Vector3 originalDirection, float bulletSpeed, int bulletDamage)
    {
        transform.forward = originalDirection;
        rb.velocity = transform.forward * bulletSpeed;
        this.bulletDamage = bulletDamage;
    }

    private void OnCollisionEnter(Collision other)
    {
        AudioManager.instance.Play3D(bulletSound, transform.position);
        VFXManager.Instance.PlayVFX(bulletImpactEffect, transform.position);
        
        Destroy(gameObject);
    }
}
