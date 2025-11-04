
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MultyPlayer : MonoBehaviour
{
    //serialized
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPosition;
    [SerializeField] float fireRate = 0.5f;
    [SerializeField]float bulletSpeed = 1f;
    [SerializeField] private AudioClip playerShotSound;
    [SerializeField] GameObject bulletFiringEffect;
    [SerializeField] int bulletDamage;
    

    
    
    //public
    public int health = 100;
    
    //private
    private Rigidbody rb; 
    private float nextFire = 0f;

    Slider healthBar;
    PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        healthBar = FindFirstObjectByType<Slider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        Move();
        if(Input.GetMouseButtonDown(0))
        {
            photonView.RPC("Fire", RpcTarget.AllViaServer);
        }
    }

    private void Update()
    {
        /*if(Input.GetMouseButtonDown(0))
        {
            Fire();
        }*/
    }
    [PunRPC]
    private void Fire()
    {
        if (Time.time >= nextFire)
        {
            nextFire = Time.time + fireRate;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition.position, Quaternion.identity);
            var bulletDir = transform.rotation * Vector3.forward;
            bullet.GetComponent<MultiplayerBulletController>()?.InitializeBullet(bulletDir, bulletSpeed, bulletDamage);
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
     void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            MultiplayerBulletController bulletController = collision.gameObject.GetComponent<MultiplayerBulletController>();
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
            PlayerDied();
        }
    }

    private void PlayerDied()
    {
        gameObject.SetActive(false);
    }
}