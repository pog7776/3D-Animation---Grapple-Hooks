using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    public James_Controller james;
    public string command;
    public CameraController cc;
    public string Ccommand;
    public Transform cPos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "James" || other.tag == "Hook")
        {
            CommandJames(command);
            CommandCamera(Ccommand);
            gameObject.SetActive(false);
        }
    }

    private void CommandCamera(string c) {
        switch(c){
            case "FollowJames":
                cc.FollowTarget(james.GetJames(), cPos.position); break;
            case "FollowRightHook":
                cc.FollowTarget(james.GetRightHook(), cPos.position); break;
            case "FollowLeftHook":
                cc.FollowTarget(james.GetLeftHook(), cPos.position); break;
            case "WatchLookAt":
                cc.Watch(james.GetLookAt()); break;
            case "SetAt":
                cc.SetAt(cPos); break;
            case "Stop":
                cc.Stop(); break;
            case "Block":
                cc.Block(); break;
            default: break;
        }
    }

    private void CommandJames(string c) {
        switch (c)
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
            case "PlayFirstSwing":
                james.PlayTimeLine(0); break;
            case "PlayBirdHit":
                james.PlayTimeLine(1); break;
            case "PlayRoofSwing":
                james.PlayTimeLine(2); break;
            case "Pause":
                james.PauseTimeLine(); break;
            case "Resume":
                james.ResumetimeLine(); break;
            case "Stop":
                james.StopTimeLine(); break;
            default: break;
        }
    }
}
