using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    public James_Controller james;
    public string command;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "James")
        {
            switch (command)
            {
                case "AimRight":
                    james.RightArmAim(); break;
                case "FireRight":
                    james.FireRightHook(); break;
                case "AimLeft":
                    james.LeftArmAim(); break;
                case "FireLeft":
                    james.FireLeftHook(); break;
                case "UnaimRight":
                    james.RightArmUnaim(); break;
                case "UnaimLeft":
                    james.LeftArmUnaim(); break;
                case "ReelRight":
                    james.ReelRightHook(); break;
                case "ReelLeft":
                    james.ReelLeftHook(); break;
                case "Look":
                    james.Look(); break;
                case "DontLook":
                    james.DontLook(); break;
                default: break;
            }
        }
    }
}
