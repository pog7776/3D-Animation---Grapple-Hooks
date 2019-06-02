using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class JamesController : MonoBehaviour
{
    private Animator anim;
    Rigidbody rb;
    MeshCollider bodyCollider;
    private PlayableDirector playableDirector;
    private TimelineAsset timelineAsset;
    private AnimatorClipInfo[] clipInfo;

    [SerializeField] private GameObject chair;
    [SerializeField] private GameObject table;

    // Start is called before the first frame update
    void Start()
    {
        bodyCollider = gameObject.GetComponent<MeshCollider>();
        anim = gameObject.GetComponent<Animator>();
        playableDirector = GetComponent<PlayableDirector>();
        timelineAsset = (TimelineAsset) playableDirector.playableAsset;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(playableDirector.playableAsset.name);
        //Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[anim.GetCurrentAnimatorClipInfo(0).Length].clip);
        clipInfo = anim.GetCurrentAnimatorClipInfo(0);
        Debug.Log(clipInfo[0].clip);

        if(playableDirector.playableAsset.name == "Get Up"){
            chair.GetComponent<Rigidbody>().AddTorque(new Vector3(0, 0, 1));
        }
    }
}
