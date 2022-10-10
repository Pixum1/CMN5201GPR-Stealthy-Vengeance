using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiIdleState : AIState
{
    private float currTime;

    private Action switchIdleState;
    
    public SamuraiIdleState(AIController _controller, EnemyData _enemy, Action _switchIdleState) : base(_controller, _enemy)
    {
        this.switchIdleState = _switchIdleState;
    }

    public override void Enter()
    {
        currTime = enemy.IdleTime;
    }
    public override void Update()
    {
        if (currTime <= 0)
            switchIdleState.Invoke();
        else
            currTime -= Time.deltaTime;
    }
    public override void Exit()
    {

    }
}
