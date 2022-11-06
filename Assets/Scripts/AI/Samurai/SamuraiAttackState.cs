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
        shootCooldownTimer = controller.ProjectilePrefab.Cooldown;
    }
    public override void Update()
    {
        shootCooldownTimer -= Time.deltaTime;

        if (Vector2.Distance(controller.transform.position, PlayerController.Instance.transform.position) <= controller.AttackRange)
        {
            if (shootCooldownTimer <= 0)
            {
                Projectile p = GameObject.Instantiate(controller.ProjectilePrefab, controller.transform.position, Quaternion.identity);
                p.Launch(PlayerController.Instance.transform.position - controller.transform.position, bulletLifeTime, "Enemy");

                shootCooldownTimer = controller.ProjectilePrefab.Cooldown;
            }
        }
        else
        {
            //Vector2 destination = new Vector2(PlayerController.Instance.transform.position.x, controller.transform.position.y);

            //Vector2 rightPoint = new Vector3(controller.transform.position.x + 2, controller.transform.position.y - controller.transform.localScale.y / 2);
            //Vector2 leftPoint = new Vector3(controller.transform.position.x - 2, controller.transform.position.y - controller.transform.localScale.y / 2);

            //Collider2D[] rightCol = Physics2D.OverlapBoxAll(rightPoint, Vector2.one, 0f, LayerMask.GetMask("Obstacle"));
            //Collider2D[] leftCol = Physics2D.OverlapBoxAll(leftPoint, Vector2.one, 0f, LayerMask.GetMask("Obstacle"));

            //if (leftCol.Length > 0 && rightCol.Length > 0)
            //controller.transform.position = Vector3.MoveTowards(controller.transform.position, destination, controller.Speed * Time.deltaTime);

            controller.transform.position = Vector3.MoveTowards(controller.transform.position, new Vector3(PlayerController.Instance.transform.position.x, controller.transform.position.y), controller.Speed * Time.deltaTime);
        }
    }
    public override void Exit()
    {

    }
}
