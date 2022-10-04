using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState
{
    protected AIController controller;
    protected Enemy enemy;

    public AIState(AIController _controller, Enemy _enemy)
    {
        this.controller = _controller;
        this.enemy = _enemy;
    }
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
