using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookAim : MonoBehaviour
{
    public bool canAim = true;
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        if (canAim) {
            transform.LookAt(target.position);
        }
        
    }
}
