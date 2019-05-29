using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingController : MonoBehaviour
{
    public HookController hook;
    public HookAim aim;
    public Transform hand;

    public void Fire() {
        hook.Fire();
    }

    public void Reel() {
        hook.Reel();
    }

    public void NoAim() {
        aim.canAim = false;
    }

    public void Aim() {
        aim.canAim = true;
    }

    private void FixedUpdate()
    {
        transform.position = hand.position;
    }
}
