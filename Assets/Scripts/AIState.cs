using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState
{
    protected AIController controller;

    public AIState(AIController _controller)
    {
        this.controller = _controller;
    }
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
