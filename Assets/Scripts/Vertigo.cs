using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertigo : MonoBehaviour
{
    private Camera cam;
    [SerializeField] float vertiSpeed = 0.5f;
    private Vector3 thrustDirection;
    [SerializeField] GameObject head;
    

    // Start is called before the first frame update
    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        //head.transform.rotation =  new Quaternion(-55, 6.259f, -9.976001f, 0);
        //Vector3 headLocation =  FindTarget(head);
        //transform.localPosition -= new Vector3(headLocation.x , headLocation.y, headLocation.z).normalized * - 0.02f;
        //transform.localPosition -= new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, head.transform.position.z * - 0.02f).normalized;
        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, gameObject.transform.position.z - 0.02f);
        cam.fieldOfView += vertiSpeed;
    }

    private Vector3 FindTarget(GameObject target) {
        //...setting thurst direction
        thrustDirection = target.transform.position;
        thrustDirection.z = 0.0f;
        thrustDirection = thrustDirection - transform.position;
    
        return thrustDirection;
    }
}
