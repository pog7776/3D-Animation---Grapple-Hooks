using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : MonoBehaviour
{

    public GameObject red;
    public GameObject blue;

    public float lightTime = 0.4f;
    private float lightTimer;

    // Start is called before the first frame update
    void Start()
    {
        lightTimer = lightTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(lightTimer > 0){
            lightTimer -= Time.deltaTime;
        }
        else {
            lightTimer = lightTime;
            switchLights();
        }
    }

    void switchLights() {
        if (red.active) {
            red.SetActive(false);
            blue.SetActive(true);
        }
        else {
            red.SetActive(true);
            blue.SetActive(false);
        }
    }
}
