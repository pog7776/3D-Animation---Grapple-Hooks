using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookLine : MonoBehaviour
{
    public Transform start;
    public Transform end;

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(start.position, end.position, Color.black);
    }
}
