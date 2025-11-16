using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviourPunCallbacks
{
    private const string TargetSceneName = "Multiplayer";

    public override void OnEnable()
    {
        base.OnEnable();

        StartCoroutine(nameof(LoadTargetScene));
    }

    private IEnumerator LoadTargetScene()
    {
        yield return new WaitForSeconds(2);
        if (PhotonNetwork.IsMasterClient)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(TargetSceneName);
            }
            
        }
    }
}
