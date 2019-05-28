using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_LeftArm : MonoBehaviour
{
    public Animator anime;
    public Transform target;
    public float weight;
    public Transform joint;
    public float jointWeight;
    public bool canIK;

    private void Awake()
    {
        anime = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (canIK)
        {
            anime.SetIKPosition(AvatarIKGoal.LeftHand, target.position);
            anime.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
            anime.SetIKHintPosition(AvatarIKHint.LeftElbow, joint.position);
            anime.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, jointWeight);
        }
    }
}
