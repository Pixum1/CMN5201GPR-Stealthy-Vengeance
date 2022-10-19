using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SamuraiRoamingState : AIState
{
    private Action switchIdleState;
    private Vector2 targetDestination;
    private float roamExitTime = 10f;
    private float roamTimer;

    public SamuraiRoamingState(AIController _controller, EnemyData _enemy, Action _switchIdleState) : base(_controller, _enemy)
    {
        this.switchIdleState = _switchIdleState;
    }

    public override void Enter()
    {
        roamTimer = roamExitTime;

        targetDestination = controller.transform.position;

        if (enemy.Waypoints == null)
            switchIdleState.Invoke();
        else
        {
            if (enemy.Waypoints.Length > 0)
                targetDestination = enemy.Waypoints[UnityEngine.Random.Range(0, enemy.Waypoints.Length)];
        }
    }
    public override void Update()
    {
        if (((Vector2)controller.transform.position - targetDestination).sqrMagnitude <= 0.05f)
            switchIdleState.Invoke();
        else
            controller.transform.position = Vector3.MoveTowards(controller.transform.position, targetDestination,
                enemy.Speed * Time.deltaTime);

        if (roamTimer <= 0)
            switchIdleState.Invoke();

        roamTimer -= Time.deltaTime;
    }
    public override void Exit()
    {

    }
}