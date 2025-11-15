
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;

public class MultyPlayer : MonoBehaviour, IPunObservable
{
    //serialized
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPosition;
    [SerializeField] float fireRate = 0.5f;
    [SerializeField]float bulletSpeed = 1f;
    [SerializeField] Slider healthBar;
    [SerializeField] private AudioClip playerShotSound;
    [SerializeField] GameObject bulletFiringEffect;
    [SerializeField] int bulletDamage;
    

    public float moveSpeed = 5f; 
    public float rotationSmoothTime = 0.05f; 
    private float rotationVelocity;
    
    //public
    public int health = 100;
    
    //private
    private Rigidbody rb; 
    private float nextFire = 0f;

    Slider mainHealthbar;
    PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.SetCursorState(false);
        
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        mainHealthbar = FindFirstObjectByType<Slider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        
       
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        Move();
        if (Input.GetMouseButtonDown(0))
        {
            photonView.RPC("Fire", RpcTarget.AllViaServer);
        }
    }
    [PunRPC]
    private void Fire()
    {
        if (Time.time >= nextFire)
        {
            nextFire = Time.time + fireRate;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition.position, Quaternion.identity);
            var bulletDir = transform.rotation * Vector3.forward;
            bullet.GetComponent<MultiplayerBulletController>()?.InitializeBullet(bulletDir, bulletSpeed, bulletDamage, photonView.Owner );
            AudioManager.instance.Play3D(playerShotSound, transform.position);
            
            VFXManager.Instance.PlayVFX(bulletFiringEffect, bulletSpawnPosition.position, bulletDir);
        }
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); 
        float verticalInput = Input.GetAxis("Vertical");   

        Vector3 inputDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (inputDirection.magnitude < 0.1f)
        {
            // If no movement input, just return
            return;
        }


        if (Camera.main != null)
        {
            float targetAngle = Camera.main.transform.eulerAngles.y;

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, 
                targetAngle, 
                ref rotationVelocity, 
                rotationSmoothTime);
        
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        
        Vector3 moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    
    }
    private void OldMove()
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
                TakeDamage(bulletController);
            }
        }
    }

    private void TakeDamage(MultiplayerBulletController bulletController)
    {
        health -= bulletController.bulletDamage;
        healthBar.value = health;
        mainHealthbar.value = health;
        if (health <= 0)
        {
            bulletController.Owner.AddScore(1);
            PlayerDied();
        }
    }

    private void PlayerDied()
    {
        health = 100;
        healthBar.value = health;
        //gameObject.SetActive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (int)stream.ReceiveNext();
            healthBar.value=health;
        }
    }
}