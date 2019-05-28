using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightGrip : MonoBehaviour
{
    public Transform targetLow;
    public Transform targetHigh;
    public float spacing;

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = (targetHigh.position + targetLow.position) / 2;
        transform.position = newPos;
        transform.LookAt(newPos);
        transform.position -= transform.forward * spacing;
    }
}
