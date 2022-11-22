using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public delegate bool StateMachineSwitchDelegate();

public abstract class AIController : MonoBehaviour, IAIControlls
{
    // Internal variables
    protected Dictionary<AIState, Dictionary<StateMachineSwitchDelegate, AIState>> StateDictionary;
    protected bool CurrIdleState;
    protected AIState CurrentState;
    public bool SeesPlayer;
    private Vector2 prevMoveDir = Vector2.zero;
    public float IdleTime;
    public float Speed;
    public float AttackRange;
    public float VisionRange;

    // References
    public Animator Anim;
    public Health Health;
    public SpriteRenderer SpriteRenderer;
    public Rigidbody2D rb;
    [HideInInspector] public EnemyData EnemyData;
    public AIPath Path;
    public Projectile ProjectilePrefab;

    public Drops[] Drops;

    public LayerMask ObstacleLayer;

    private void Start()
    {
        InitializeFSM();
    }
    private void Update()
    {
        UpdateLogic();
        UpdateFSM();
    }

    /// <summary>
    /// Create new States and conditions here.
    /// </summary>
    public abstract void InitializeFSM();
    public void UpdateFSM()
    {
        foreach (var transition in StateDictionary[CurrentState])
        {
            if (transition.Key())
            {
                //Debug.Log($"Switched from {CurrentState} to {transition.Value}!");

                CurrentState.Exit();
                transition.Value.Enter();
                CurrentState = transition.Value;

                break;
            }
        }

        CurrentState.Update();
    }
    public virtual void UpdateLogic()
    {
        if (prevMoveDir.x < transform.position.x)
            SpriteRenderer.flipX = false;
        else if (prevMoveDir.x > transform.position.x)
            SpriteRenderer.flipX = true;

        prevMoveDir = transform.position;

        if (PlayerController.Instance == null) return;

        float range = (transform.position - PlayerController.Instance.transform.position).sqrMagnitude;

        if (range <= Mathf.Pow(VisionRange, 2))
        {
            Ray2D ray = new Ray2D(transform.position, PlayerController.Instance.transform.position - transform.position);

            if (!Physics2D.Raycast(ray.origin, ray.direction, Vector2.Distance(transform.position, PlayerController.Instance.transform.position), LayerMask.GetMask("Obstacle")))
            {
                Debug.DrawRay(ray.origin, ray.direction * Vector2.Distance(transform.position, PlayerController.Instance.transform.position), Color.red);
                SeesPlayer = true;
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * Vector2.Distance(transform.position, PlayerController.Instance.transform.position), Color.yellow);
                SeesPlayer = false;
            }
        }
        else
        {
            SeesPlayer = false;
        }

    }
    public void SwitchIdleState()
    {
        CurrIdleState = !CurrIdleState;
    }
}
