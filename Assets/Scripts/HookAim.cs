using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookAim : MonoBehaviour
{
    public bool canAim = true;
    public Transform target;
    public Transform rightHand;
    public Transform leftHand;
    public bool right;

    // Update is called once per frame
    void Update()
    {
        if (right)
        {
            transform.position = rightHand.position;
            transform.localRotation = rightHand.localRotation;
        }
        else {
            transform.position = leftHand.position;
            transform.localRotation = leftHand.localRotation;
        }
        if (canAim) {
            transform.LookAt(target.position);
        }
        
    }
}
