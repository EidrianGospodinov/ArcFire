
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
    [SerializeField] int maxHealth = 100;
    
    //public
    public float moveSpeed = 5f; 
    public float rotationSmoothTime = 0.05f; 
    
    public Transform weaponAimPivot; 
    public float verticalAimSpeed = 50f;
    public float maxAimAngle = 80f;     
    public float minAimAngle = -45f;    
    
    //private
    private float rotationVelocity;
    private int health = 100;
    private Rigidbody rb; 
    private float nextFire = 0f;

    private UiHealthBar mainHealthbar;
    private PhotonView photonView;
    public PhotonView PV => photonView;
    
    private float currentVerticalAim = 0f;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.SetCursorState(true);
        
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        mainHealthbar = GameObject.FindWithTag("HealthBarMain")?.GetComponent<UiHealthBar>();

        health = maxHealth;
        if (photonView.IsMine)
        {
            healthBar.gameObject.SetActive(false);
        }
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
        if (GameManager.Instance.isGamePausedOrOver)
        {
            return;
        }
        if (!photonView.IsMine)
        {
            return;
        }
        Move();
        HandleWeaponAimInput();
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
            
            var bulletDir = bulletSpawnPosition.forward;
            
            // Set the bullet's rotation to match its trajectory
            bullet.transform.rotation = Quaternion.LookRotation(bulletDir);
            
            bullet.GetComponent<MultiplayerBulletController>()?.InitializeBullet(bulletDir, bulletSpeed, bulletDamage, photonView.Owner );
            
            
            
            AudioManager.instance.Play3D(playerShotSound, transform.position);
            
            VFXManager.Instance.PlayVFX(bulletFiringEffect, bulletSpawnPosition.position, bulletDir);
        }
    }
    private void HandleWeaponAimInput()
    {
        if (weaponAimPivot == null)
        {
            return;
        }
        float input = 0f;
        // Aim Up
        if (Input.GetKey(KeyCode.Q))
        {
            input = 1f;
        }
        // Aim Down
        else if (Input.GetKey(KeyCode.E))
        {
            input = -1f;
        }
        if (input != 0)
        {
            //Calculate the change in vertical aim based on input and time
            currentVerticalAim += input * verticalAimSpeed * Time.deltaTime;
        
            //Clamp the angle to prevent the weapon from rotating too far
            currentVerticalAim = Mathf.Clamp(currentVerticalAim, minAimAngle, maxAimAngle);
        }
        //Apply the stored angle to the weapon pivot's local rotation
        weaponAimPivot.localRotation = Quaternion.Euler(currentVerticalAim, 0f, 0f);
    }
    private void OldFire()
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
                if (Equals(bulletController.Owner, photonView.Owner))
                {
                    return;
                }
                TakeDamage(bulletController);
            }
        }
    }

    private void TakeDamage(MultiplayerBulletController bulletController)
    {
        health -= bulletController.bulletDamage;
        UpdateHealthUI();
        if (health <= 0)
        {
            bulletController.Owner.AddScore(1);
            PlayerDied();
        }
    }

    private void PlayerDied()
    {
        UpdateHealthUI(true);
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
    public void UpdateHealthUI(bool isPlayerDead = false) 
    {
        if (isPlayerDead)
        {
            health = maxHealth;
        }
        float percentage = (float)health / maxHealth;
        healthBar.value = health; 
        
        if (photonView.IsMine)
        {
            mainHealthbar.SetHealthBarPercentage(percentage);
        }
    }
}