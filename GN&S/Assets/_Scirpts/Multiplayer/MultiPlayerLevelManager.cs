using Photon.Pun;
using UnityEngine;

public class MultiPlayerLevelManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PhotonNetwork.Instantiate("MultiPlayerPref", Vector3.zero, Quaternion.identity);
    }

   
}
