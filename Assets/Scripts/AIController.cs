using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate bool StateMachineSwitchDelegate();

public class AIController : MonoBehaviour
{
    private bool currIdleState;
    private AIState currentState;
    public Health m_Health;

    private Dictionary<AIState, Dictionary<StateMachineSwitchDelegate, AIState>> stateDictionary;

    private void Start()
    {
        if (m_Health == null)
            m_Health = GetComponent<Health>();

        InitializeFSM();
    }

    private void Update()
    {
        UpdateFSM();
    }

    private void InitializeFSM()
    {
        // Create new States here
        IdleState idleState = new IdleState(this, SwitchIdleState);
        RoamingState roamingState = new RoamingState(this, SwitchIdleState);
        AttackState attackState = new AttackState(this, SwitchIdleState);
        DeathState deathState = new DeathState(this, SwitchIdleState);

        currentState = idleState;

        stateDictionary = new Dictionary<AIState, Dictionary<StateMachineSwitchDelegate, AIState>>()
        {
            {
                idleState,
                new Dictionary<StateMachineSwitchDelegate, AIState>
                {
                    {()=> !currIdleState, roamingState },
                    {()=> m_Health.HP <= 0, deathState },
                }
            },
            {
                roamingState,
                new Dictionary<StateMachineSwitchDelegate, AIState>
                {
                    {()=> currIdleState, idleState },
                    {()=> m_Health.HP <= 0, deathState },
                }
            },
            {
                attackState,
                new Dictionary<StateMachineSwitchDelegate, AIState>
                {
                    {()=> false, idleState },
                    {()=> m_Health.HP <= 0, deathState },
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

    private void UpdateFSM()
    {
        foreach (var transition in stateDictionary[currentState])
        {
            if (transition.Key())
            {
                currentState.Exit();
                transition.Value.Enter();
                currentState = transition.Value;

                break;
            }
        }

        currentState.Update();
    }

    private void SwitchIdleState()
    {
        currIdleState = !currIdleState;
    }
}
