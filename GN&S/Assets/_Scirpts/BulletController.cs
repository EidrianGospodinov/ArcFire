using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] AudioClip bulletSound;
    [SerializeField] private GameObject bulletImpactEffect;
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

    public void InitializeBullet(Vector3 originalDirection, float bulletSpeed )
    {
        transform.forward = originalDirection;
        rb.velocity = transform.forward * bulletSpeed;
        
    }

    private void OnCollisionEnter(Collision other)
    {
        AudioManager.instance.Play3D(bulletSound, transform.position);
        VFXManager.Instance.PlayVFX(bulletImpactEffect, transform.position);
        
        Destroy(gameObject);
    }
}
