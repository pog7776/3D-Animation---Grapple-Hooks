using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookController : MonoBehaviour
{
    public Transform target;
    public GameObject hook;
    public float speed = 0.1f;
    public bool canFire = true;
    public bool reached = false;
    public Vector3 dir;
    public bool control = true;

    private void Awake()
    {
        dir = Vector3.zero;
    }

    private void Update()
    {
        if (control) {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (canFire && !reached)
                {
                    canFire = false;
                    dir = (target.position - hook.transform.position).normalized;
                }
                else if (!canFire && reached)
                {
                    dir = -(target.position - hook.transform.position).normalized;
                }
            }
            hook.transform.position += dir * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (control) {
            if (!canFire && !reached)
            {
                if (other.tag == "Target")
                {
                    reached = true;
                    dir = Vector3.zero;
                }
            }
            if (!canFire && reached)
            {
                if (other.tag == "Barrel")
                {
                    canFire = true;
                    reached = false;
                    dir = Vector3.zero;
                }
            }
        }
    }
}
