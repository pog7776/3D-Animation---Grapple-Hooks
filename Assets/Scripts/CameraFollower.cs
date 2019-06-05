using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform target;
    public float offSet;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        transform.position = target.position + Vector3.left * offSet;
    }
}
