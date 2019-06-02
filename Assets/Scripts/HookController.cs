using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookController : MonoBehaviour
{
    public float speed = 0.1f;
    public Transform barrel;
    public Transform target;
    public bool fired;
    public bool reeling;
    public bool landed;

    public void Fire()
    {
        if (!fired) {
            fired = true;
            StartCoroutine(FireHook());
        }
    }

    IEnumerator FireHook() {
        while (fired) {
            transform.position += (target.position - transform.position).normalized * speed;
            yield return null;
        }
    }

    public void Reel()
    {
        if (!reeling) {
            reeling = true;
            landed = false;
            StartCoroutine(ReelHook());
        }
    }

    IEnumerator ReelHook() {
        while (reeling) {
            transform.position += (barrel.position - transform.position).normalized * speed;
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (fired) {
            if (other.tag == "Target") {
                fired = false;
                StopCoroutine(FireHook());
                landed = true;
            }
        }
        if (reeling) {
            if (other.tag == "Barrel") {
                reeling = false;
                StopCoroutine(ReelHook());
            }
        }
    }

    private void Update()
    {
        if (landed) {
            transform.position = target.position;
        }
    }

    public Transform GetHook() {
        return transform;
    }
}
