using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_RightLeg : MonoBehaviour
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
            anime.SetIKPosition(AvatarIKGoal.RightFoot, target.position);
            anime.SetIKPositionWeight(AvatarIKGoal.RightFoot, weight);
            anime.SetIKHintPosition(AvatarIKHint.RightKnee, joint.position);
            anime.SetIKHintPositionWeight(AvatarIKHint.RightKnee, jointWeight);
        }
    }
}
