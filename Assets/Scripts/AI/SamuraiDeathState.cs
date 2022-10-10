using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SamuraiDeathState : AIState
{
    private Action switchIdleState;

    public SamuraiDeathState(AIController _controller, EnemyData _enemy, Action _switchIdleState) : base(_controller, _enemy)
    {
        this.switchIdleState = _switchIdleState;
        _controller.Health.E_TriggerDeath += Die;
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

    private void Die()
    {
        controller.SpriteRenderer.color = Color.red;

        Debug.LogWarning("Enemy Died!");
    }
}
