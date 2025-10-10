using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void PlayVFX(GameObject effectObject, Vector3 position, Vector3 bulletDir = new Vector3())
    {
        GameObject vfxObject = Instantiate(effectObject, position,  Quaternion.identity);
        vfxObject.transform.rotation = Quaternion.LookRotation(bulletDir*-1);
        
        ParticleSystem[] particleSystem = vfxObject.GetComponentsInChildren<ParticleSystem>();
        float maxLength = 0f;
        foreach (var individualParticles in particleSystem)
        {
            float currentKnownMaxLength = individualParticles.main.duration + individualParticles.main.startLifetime.constant;
            if (currentKnownMaxLength > maxLength)
            {
                maxLength = currentKnownMaxLength;
            }
        }
        Destroy(vfxObject, maxLength);

    }
}
