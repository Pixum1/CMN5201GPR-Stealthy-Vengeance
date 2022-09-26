using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : AIState
{
    private Action switchIdleState;
    
    public IdleState(AIController _controller, Action _switchIdleState) : base(_controller)
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
