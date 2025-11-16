using Cinemachine;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCameraSetup : MonoBehaviour
{
    PhotonView photonView;

    private bool isFirstGameLoop = true;
    private CinemachineFreeLook cineMachine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFirstGameLoop && photonView.IsMine)
        {
            var cam = Camera.main.GameObject();
            if (cam != null)
            {
                cineMachine = cam.GetComponent<CinemachineFreeLook>();
                cineMachine.Follow = this.transform;
                cineMachine.LookAt = this.transform;
            }
            isFirstGameLoop = false;
            return;
        }

        if (cineMachine != null)
        {
            cineMachine.enabled = !GameManager.Instance.isGamePausedOrOver;
        }
    }
}
