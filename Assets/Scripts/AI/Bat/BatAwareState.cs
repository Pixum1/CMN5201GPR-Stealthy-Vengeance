using System;
using System.Collections.Generic;
using UnityEngine;

public class BatAwareState : AIState
{
    public Action switchIdleState;
    public BatAwareState(AIController _controller, EnemyData _data, Action _switchIdleState) : base(_controller, _data)
    {
        switchIdleState = _switchIdleState;
    }
    public override void Enter()
    {
        switchIdleState.Invoke();
    }

    public override void Exit()
    {

    }

    public override void Update()
    {

    }
}
