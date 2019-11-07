using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour {

    protected SoccerPlayer _sp;
    public virtual void OnEnterState()
    {
        if (_sp == null)
            _sp = GetComponent<SoccerPlayer>();
    }
    public abstract void Action();
    public virtual void OnExitState()
    {
        Destroy(this);
    }
}
