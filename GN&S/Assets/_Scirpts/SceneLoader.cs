using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] string SceneName;
    public void LoadScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
