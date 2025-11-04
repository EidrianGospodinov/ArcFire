using Photon.Pun;
using UnityEngine;

public class MultiPlayerLevelManager : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PhotonNetwork.Instantiate("MultiPlayerPref", playerTransform.position, Quaternion.identity);
    }

   
}
