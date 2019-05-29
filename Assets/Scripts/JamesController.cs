using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JamesController : MonoBehaviour
{
    private Animator anim;
    Rigidbody rb;
    CapsuleCollider bodyCollider;
    [SerializeField] float relaxTime = 8;

    void Awake()
    {
        //rb =  gameObject.GetComponent<Rigidbody>();
        //rb.detectCollisions = true;
        //rb.useGravity = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        bodyCollider = gameObject.GetComponent<CapsuleCollider>();
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Relax(relaxTime));
    }

    private IEnumerator Relax(float time){
        yield return new WaitForSeconds(time);
        bodyCollider.enabled = true;
        anim.SetBool("relaxing", false);
        //rb.detectCollisions = true;
        //rb.useGravity = true;
    }
}
