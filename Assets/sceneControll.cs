﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneControll : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("r")){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if(Input.GetKeyDown("1")){
            SceneManager.LoadScene(0);
        }
        if(Input.GetKeyDown("2")){
            SceneManager.LoadScene(1);
        }
        if(Input.GetKeyDown("3")){
            SceneManager.LoadScene(2);
        }
        if(Input.GetKeyDown("4")){
            SceneManager.LoadScene(3);
        }
        if(Input.GetKeyDown("5")){
            SceneManager.LoadScene(4);
        }
        if(Input.GetKeyDown("6")){
            SceneManager.LoadScene(5);
        }
        if(Input.GetKeyDown("7")){
            SceneManager.LoadScene(6);
        }
        
    }
}
