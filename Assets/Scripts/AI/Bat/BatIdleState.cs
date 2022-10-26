using System;
using System.Collections.Generic;
using UnityEngine;

public class BatIdleState : AIState
{
    private Action switchIdleState;
    public BatIdleState(AIController _controller, EnemyData _enemy, Action _switchIdleState) : base(_controller, _enemy)
    {
        this.switchIdleState = _switchIdleState;
    }
    public override void Enter()
    {
        controller.Path.maxSpeed = data.Speed;
        controller.Path.destination = (Vector2)data.SpawnPoint;
    }

    public override void Exit()
    {

    }

    public override void Update()
    {
        if (controller.Path.reachedDestination)
        {
            switchIdleState.Invoke();
        }
    }
}
