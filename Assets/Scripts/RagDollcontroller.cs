using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDollcontroller : MonoBehaviour
{
    public Rigidbody rb;
    public float force;
    public Vector3 dir;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            rb.AddForce(dir*force, ForceMode.Impulse);
        }
    }
}
