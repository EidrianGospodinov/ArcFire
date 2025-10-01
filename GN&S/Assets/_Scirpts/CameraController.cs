using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Vector3 offset;
    [SerializeField] Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerTransform.position + offset;
    }
}
