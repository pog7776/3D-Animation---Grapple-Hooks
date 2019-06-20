using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDollcontroller : MonoBehaviour
{
    public Rigidbody[] rbs;
    public Rigidbody target;
    public float force;
    public Vector3 dir;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            target.AddForce(dir*force, ForceMode.Impulse);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Finish") {
            for (int i = 0; i < rbs.Length; i++) {
                rbs[i].velocity = Vector3.zero;
            }
        }
    }
}
