using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIControlls
{
    void InitializeFSM();
    void UpdateFSM();
    void SwitchIdleState();
    void UpdateLogic();
}
