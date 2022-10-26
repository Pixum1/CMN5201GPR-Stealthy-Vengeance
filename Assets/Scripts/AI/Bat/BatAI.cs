using System;
using System.Collections.Generic;
using UnityEngine;
public class BatAI : AIController
{
    public override void UpdateLogic()
    {
        base.UpdateLogic();
    }
    public override void InitializeFSM()
    {
        BatIdleState idleState = new BatIdleState(this, EnemyData, SwitchIdleState);
        BatAttackState attackState = new BatAttackState(this, EnemyData, SwitchIdleState);
        BatDeathState deathState = new BatDeathState(this, EnemyData, SwitchIdleState);
        BatAwareState awareState = new BatAwareState(this, EnemyData, SwitchIdleState);

        CurrentState = idleState;

        StateDictionary = new Dictionary<AIState, Dictionary<StateMachineSwitchDelegate, AIState>>
        {
            {
                idleState,
                new Dictionary<StateMachineSwitchDelegate, AIState>
                {
                    {()=> Health.HP <= 0, deathState},
                    {()=> Path.reachedDestination || !Path.hasPath, awareState },
                }
            },
            {
                attackState,
                new Dictionary<StateMachineSwitchDelegate, AIState>
                {
                    {()=> Health.HP <= 0, deathState},
                    {()=> !CurrIdleState, idleState },
                    {()=> !SeesPlayer && Path.reachedDestination, idleState},
                }
            },
            {
                awareState,
                new Dictionary<StateMachineSwitchDelegate, AIState>
                {
                    {()=> Health.HP <= 0, deathState},
                    {()=> SeesPlayer, attackState},
                }
            },
            {
                deathState,
                new Dictionary<StateMachineSwitchDelegate, AIState>
                {
                    {()=> false, deathState },
                }
            },
        };
    }
}
