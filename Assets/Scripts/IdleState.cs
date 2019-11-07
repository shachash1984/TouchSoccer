using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    

    public override void OnEnterState()
    {
        base.OnEnterState();
        _sp.selected = false;
        _sp.SetStationaryBar(0f, 1f);
    }
    public override void Action()
    {
        return;
    }
}
