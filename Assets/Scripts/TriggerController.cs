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
                case "AimRightArm":
                    james.RightArmAim(); break;
                case "FireRight":
                    james.FireRightHook(); break;
                case "AimLeftArm":
                    james.LeftArmAim(); break;
                case "FireLeft":
                    james.FireLeftHook(); break;
                case "UnaimRightArm":
                    james.RightArmUnaim(); break;
                case "UnaimLeftArm":
                    james.LeftArmUnaim(); break;
                case "ReelRight":
                    james.ReelRightHook(); break;
                case "ReelLeft":
                    james.ReelLeftHook(); break;
                case "Look":
                    james.Look(); break;
                case "DontLook":
                    james.DontLook(); break;
                case "AimRightLeg":
                    james.RightLegAim(); break;
                case "AimLeftLeg":
                    james.LeftLegAim(); break;
                case "UnaimRightLeg":
                    james.RightLegUnaim(); break;
                case "UnaimLeftLeg":
                    james.LeftLegUnaim(); break;
                case "GravityOn":
                    james.Gravity(); break;
                case "GravityOff":
                    james.NoGravity(); break;
                default: break;
            }
        }
    }
}
