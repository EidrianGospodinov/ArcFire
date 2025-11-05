using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MultiplayerBulletController : MonoBehaviourPunCallbacks
{
    [SerializeField] AudioClip bulletSound;
    [SerializeField] private GameObject bulletImpactEffect;
    public int bulletDamage;
    
    [HideInInspector]
    public Photon.Realtime.Player Owner;
    
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

    public void InitializeBullet(Vector3 originalDirection, float bulletSpeed, int bulletDamage, Photon.Realtime.Player givenPlayer)
    {
        transform.forward = originalDirection;
        rb.linearVelocity = transform.forward * bulletSpeed;
        this.bulletDamage = bulletDamage;

        Owner = givenPlayer;
    }

    private void OnCollisionEnter(Collision other)
    {
        AudioManager.instance.Play3D(bulletSound, transform.position);
        VFXManager.Instance.PlayVFX(bulletImpactEffect, transform.position);
        
        Destroy(gameObject);
    }
}
