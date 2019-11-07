using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingState : State {

    public override void Action()
    {
        return;
    }

    public override void OnEnterState()
    {
        
        base.OnEnterState();
        StartCoroutine(_sp.Aim());
    }

    public override void OnExitState()
    {
        _sp.Shoot();
        _sp.ToggleArrow(false);
        base.OnExitState();
    }


}
