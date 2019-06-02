using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbController : MonoBehaviour
{
    public float speed;
    public Animator anime;
    public bool hit = false;

    private void FixedUpdate()
    {
        transform.position += transform.forward * speed;
        if (hit) {
            if (anime.GetCurrentAnimatorStateInfo(0).IsName("Eagle_Damaged") && anime.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0) {
                hit = false;
                anime.SetBool("Hit", false);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "James") {
            hit = true;
            anime.SetBool("Hit", true);
        }
    }
}
