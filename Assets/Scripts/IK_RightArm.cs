using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_RightArm : MonoBehaviour
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
        if (canIK) {
            anime.SetIKPosition(AvatarIKGoal.RightHand, target.position);
            anime.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
            anime.SetIKHintPosition(AvatarIKHint.RightElbow, joint.position);
            anime.SetIKHintPositionWeight(AvatarIKHint.RightElbow, jointWeight);
        }
    }
}
