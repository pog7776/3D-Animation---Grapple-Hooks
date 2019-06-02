using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

    public float moveSpeed = .5f;
    public float minX = 0;
    public float maxX = 50;
    public float minZ = 0;
    public float maxZ = 50;
	
	// Update is called once per frame
	void Update () {
        Vector3 newPos = Vector3.zero; ;
        newPos.x += Mathf.Clamp(transform.position.x + moveSpeed * Input.GetAxis("Horizontal"), minX, maxX);
        newPos.z += Mathf.Clamp(transform.position.z + moveSpeed * Input.GetAxis("Vertical"), minZ, maxZ);
        newPos.y = transform.position.y;
        transform.position = newPos;
	}
}
