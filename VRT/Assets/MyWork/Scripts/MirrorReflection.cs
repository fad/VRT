using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorReflection : MonoBehaviour
{ 
    [SerializeField]
    private Transform mirrorCam;
    [SerializeField]
    private Transform playerCam;

  
    void Update()
    {
        ClaculateRotation();
    }

    void ClaculateRotation()
    {
        Vector3 dir = (playerCam.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);

        rot.eulerAngles = transform.eulerAngles - rot.eulerAngles;

        mirrorCam.localRotation = rot;
    }
}
