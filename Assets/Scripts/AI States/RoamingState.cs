using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoamingState : AIState
{
    private Action switchIdleState;

    public RoamingState(AIController _controller, Action _switchIdleState) : base(_controller)
    {
        this.switchIdleState = _switchIdleState;
    }

    public override void Enter()
    {

    }
    public override void Update()
    {

    }
    public override void Exit()
    {

    }
}
