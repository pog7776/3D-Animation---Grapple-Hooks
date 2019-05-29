using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_LookAt : MonoBehaviour
{
    public Animator anime;
    public Transform target;
    public float weight;
    public bool canLook;

    private void Awake()
    {
        anime = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (canLook) {
            anime.SetLookAtPosition(target.position);
            anime.SetLookAtWeight(weight);
        }
    }
}
