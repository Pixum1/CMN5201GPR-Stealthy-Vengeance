using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SamuraiAttackState : AIState
{
    private Action switchIdleState;
    private float shootCooldownTimer;
    private float bulletLifeTime = 5f;

    public SamuraiAttackState(AIController _controller, EnemyData _enemy, Action _switchIdleState) : base(_controller, _enemy)
    {
        this.switchIdleState = _switchIdleState;
    }

    public override void Enter()
    {
        shootCooldownTimer = data.ShootCooldown;
    }
    public override void Update()
    {
        shootCooldownTimer -= Time.deltaTime;

        if (Vector2.Distance(controller.transform.position, PlayerController.Instance.transform.position) <= data.AttackRange)
        {
            if (shootCooldownTimer <= 0)
            {
                Projectile p = GameObject.Instantiate(data.ProjectilePrefab, controller.transform.position, Quaternion.identity);
                p.Launch(PlayerController.Instance.transform.position - controller.transform.position, data.ShootForce, bulletLifeTime, "Enemy");

                shootCooldownTimer = data.ShootCooldown;
            }
        }
        else
        {
            Vector2 destination = new Vector2(PlayerController.Instance.transform.position.x, controller.transform.position.y);

            controller.transform.position = Vector3.MoveTowards(controller.transform.position, destination,
                   data.Speed * Time.deltaTime);
        }
    }
    public override void Exit()
    {

    }
}
