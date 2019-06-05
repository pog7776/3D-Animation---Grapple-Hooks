using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public bool follow;
    public bool look;
    public Vector3 offSet;

    public void SetAt(Transform t) {
        transform.position = t.position;
        transform.rotation = t.rotation;
    }

    public void MoveTo(Vector3 pos) {
        transform.position = pos;
    }

    public void SetOffSet() {
        offSet = transform.position - target.position;
    }

    public void GiveTarget(Transform t) {
        target = t;
    }

    public void Follow() {
        follow = true;
    }

    public void DontFollow() {
        follow = false;
    }

    public void Look() {
        look = true;
    }

    public void DontLook() {
        look = false;
    }

    public void FollowTarget(Transform t, Vector3 p) {
        if (t) {
            GiveTarget(t);
            MoveTo(p);
            SetOffSet();
            Follow();
            StartCoroutine(Following());
        }
    }

    IEnumerator Following() {
        while (follow) {
            transform.LookAt(target);
            transform.position = target.position + offSet;
            yield return null;
        }
    }

    public void Stop() {
        DontFollow();
        StopCoroutine(Following());
    }

    public void Watch(Transform t) {
        if (t) {
            GiveTarget(t);
            Look();
            StartCoroutine(Watching());
        }
    }

    IEnumerator Watching() {
        while (look) {
            transform.LookAt(target);
            yield return null;
        }
    }

    public void Block() {
        DontLook();
        StopCoroutine(Watching());
    }
}
