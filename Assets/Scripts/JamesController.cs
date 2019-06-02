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

    [SerializeField] private float furnitureWait = 1.15f;
    [SerializeField] private GameObject chair;
    [SerializeField] private float chairForce = 50;
    [SerializeField] private GameObject table;
    [SerializeField] private float tableForce = 60;
    private int cycle = 0;
    private Rigidbody chairRb;
    private Rigidbody tableRb;

    [SerializeField] private GameObject window;
    [SerializeField] private float windowWait = 4.15f;

    // Start is called before the first frame update
    void Start()
    {
        bodyCollider = gameObject.GetComponent<MeshCollider>();
        anim = gameObject.GetComponent<Animator>();
        playableDirector = GetComponent<PlayableDirector>();
        timelineAsset = (TimelineAsset) playableDirector.playableAsset;

        chairRb = chair.GetComponent<Rigidbody>();
        tableRb = table.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(SmackFurniture(furnitureWait));
        StartCoroutine(BreakWindow(windowWait));

        
        //Debug.Log(playableDirector.playableAsset.name);
        //Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[anim.GetCurrentAnimatorClipInfo(0).Length].clip);
        // clipInfo = anim.GetCurrentAnimatorClipInfo(0);
        // Debug.Log(clipInfo[0].clip);

        // if(playableDirector.playableAsset.name == "Get Up"){
        //     chair.GetComponent<Rigidbody>().AddTorque(new Vector3(0, 0, 1));
        // }
    }

    private IEnumerator SmackFurniture(float waitTime){
        yield return new WaitForSeconds(waitTime);
        if(cycle < 1){
            chairRb.AddTorque(new Vector3(chairForce, 0, 0));
            chairRb.AddForce(new Vector3(chairForce*2f, 0, 0));
        }
        if(cycle < 6){
            tableRb.AddTorque(new Vector3(-tableForce, 0, 0));
        }
        cycle++;
    }

    private IEnumerator BreakWindow(float waitTime){
        yield return new WaitForSeconds(waitTime);
        window.GetComponent<Breakable>().BreakFunction();
    }
}
