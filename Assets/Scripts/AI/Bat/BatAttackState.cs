using System;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BatAttackState : AIState
{
    private Action switchIdleState;
    public BatAttackState(AIController _controller, EnemyData _data, Action _switchIdleState) : base(_controller, _data)
    {
        this.switchIdleState = _switchIdleState;
    }
    public override void Enter()
    {

    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        Collider2D[] cols = Physics2D.OverlapBoxAll(controller.transform.position, controller.transform.localScale, 0f, LayerMask.GetMask("Player"));

        if (cols.Length != 0)
        {
            // Deal damage
            PlayerController.Instance.Health.GetDamage(1);

            // Return to idle state
            switchIdleState.Invoke();
        }
        else
        {

            // Move towards player
            if (controller.SeesPlayer)
                controller.Path.destination = PlayerController.Instance.transform.position;
        }
    }
}
