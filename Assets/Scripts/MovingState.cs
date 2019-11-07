using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : State
{

    public override void Action()
    {
        Move();
    }

    public void Move()
    {
        _sp.Move();
    }
}
