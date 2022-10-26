using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState
{
    protected AIController controller;
    protected EnemyData data;
    public AIState(AIController _controller, EnemyData _enemyData)
    {
        this.controller = _controller;
        this.data = _enemyData;
    }
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
