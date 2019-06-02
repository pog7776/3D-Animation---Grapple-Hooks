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
        //if(timelineAsset.)
    }
}
