using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiAI : AIController
{
    public override void UpdateLogic()
    {
        base.UpdateLogic();
    }

    public override void InitializeFSM()
    {
        SamuraiIdleState idleState = new SamuraiIdleState(this, EnemyData, SwitchIdleState);
        SamuraiRoamingState roamingState = new SamuraiRoamingState(this, EnemyData, SwitchIdleState);
        SamuraiAttackState attackState = new SamuraiAttackState(this, EnemyData, SwitchIdleState);
        SamuraiDeathState deathState = new SamuraiDeathState(this, EnemyData, SwitchIdleState);

        CurrentState = idleState;

        StateDictionary = new Dictionary<AIState, Dictionary<StateMachineSwitchDelegate, AIState>>()
        {
            {
                idleState,
                new Dictionary<StateMachineSwitchDelegate, AIState>
                {
                    {()=> Health.HP <= 0, deathState },
                    {()=> SeesPlayer, attackState},
                    {()=> !CurrIdleState, roamingState },
                }
            },
            {
                roamingState,
                new Dictionary<StateMachineSwitchDelegate, AIState>
                {
                    {()=> Health.HP <= 0, deathState },
                    {()=> SeesPlayer, attackState},
                    {()=> CurrIdleState, idleState },
                }
            },
            {
                attackState,
                new Dictionary<StateMachineSwitchDelegate, AIState>
                {
                    {()=> Health.HP <= 0, deathState },
                    {()=> !SeesPlayer, idleState},
                }
            },
            {
                deathState,
                new Dictionary<StateMachineSwitchDelegate, AIState>
                {
                    {()=> false, deathState},
                }
            }
        };
    }
}
