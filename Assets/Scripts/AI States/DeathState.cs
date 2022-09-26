using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DeathState : AIState
{
    private Action switchIdleState;

    public DeathState(AIController _controller, Action _switchIdleState) : base(_controller)
    {
        this.switchIdleState = _switchIdleState;
        _controller.m_Health.E_TriggerDeath += Die;
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
        Debug.LogWarning("Enemy Died!");
    }
}
