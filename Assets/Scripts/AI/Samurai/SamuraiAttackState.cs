using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SamuraiAttackState : AIState
{
    private Action switchIdleState;
    private float shootCooldownTimer;
    private float bulletLifeTime = 5f;
    private float reloadTime = 2f;
    private float reloadTimer;
    private int maxAmmo = 4;
    private int ammoCount = 4;

    int run;
    int idle;

    public SamuraiAttackState(AIController _controller, EnemyData _enemy, Action _switchIdleState) : base(_controller, _enemy)
    {
        this.switchIdleState = _switchIdleState;
    }

    public override void Enter()
    {
        run = Animator.StringToHash("Run");
        idle = Animator.StringToHash("Idle");

        shootCooldownTimer = controller.ProjectilePrefab.Cooldown;
    }
    public override void Update()
    {
        if (PlayerController.Instance == null) return;

        shootCooldownTimer -= Time.deltaTime;

        if (Vector2.Distance(controller.transform.position, PlayerController.Instance.transform.position) <= controller.AttackRange)
        {
            controller.Anim.SetTrigger(idle);

            controller.rb.drag = 50;

            if (ammoCount > 0)
            {
                reloadTimer = reloadTime;
                if (shootCooldownTimer <= 0)
                {
                    Projectile p = GameObject.Instantiate(controller.ProjectilePrefab, controller.transform.position, Quaternion.identity);
                    p.Launch(PlayerController.Instance.transform.position - controller.transform.position, bulletLifeTime, LayerMask.GetMask("Enemy"));

                    shootCooldownTimer = controller.ProjectilePrefab.Cooldown;
                    ammoCount--;
                }
            }
            else
            {
                reloadTimer -= Time.deltaTime;

                if (reloadTimer <= 0)
                    ammoCount = maxAmmo;
            }
        }
        else
        {
            controller.Anim.SetTrigger(run);

            controller.rb.drag = 0;
            int dir = Mathf.CeilToInt(Mathf.Sign(PlayerController.Instance.transform.position.x - controller.transform.position.x));

            controller.rb.AddForce(Vector2.right * dir * controller.Speed);

            if (Mathf.Abs(controller.rb.velocity.x) > controller.Speed)
                controller.rb.velocity = new Vector2(Mathf.Sign(controller.rb.velocity.x) * controller.Speed, controller.rb.velocity.y); //Clamp velocity when max speed is reached!
        }
    }
    public override void Exit()
    {

    }
}
