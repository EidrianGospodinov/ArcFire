using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EndGameCanvas : MonoBehaviour
{
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private Text whoCanStartGameText;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var isMasterClient = PhotonNetwork.IsMasterClient;
        newGameButton.SetActive(true);
        newGameButton.GetComponent<Button>().enabled = isMasterClient;
        whoCanStartGameText.enabled = !isMasterClient;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
