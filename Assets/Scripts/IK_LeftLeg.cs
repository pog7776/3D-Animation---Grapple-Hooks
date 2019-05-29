using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_LeftLeg : MonoBehaviour
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
            anime.SetIKPosition(AvatarIKGoal.LeftFoot, target.position);
            anime.SetIKPositionWeight(AvatarIKGoal.LeftFoot, weight);
            anime.SetIKHintPosition(AvatarIKHint.LeftKnee, joint.position);
            anime.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, jointWeight);
        }
    }
}
