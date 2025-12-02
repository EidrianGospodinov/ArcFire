using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] GameObject audioPrefab;
    public static AudioManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Play3D(AudioClip clip, Vector3 position)
    {
        GameObject audioObject = Instantiate(audioPrefab, position, Quaternion.identity);
        AudioSource audioSource = audioObject.GetComponent<AudioSource>();
        
        audioSource.clip = clip;
        audioSource.Play();
        Destroy(audioObject, clip.length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
