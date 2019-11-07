using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

    #region Fields
    public Team team;
    [SerializeField] private float _goalEventDelay = 1f;
    #endregion

    #region MonoBehaviour Callbacks
    IEnumerator OnTriggerEnter(Collider col)
    {
        yield return new WaitForSeconds(_goalEventDelay);
        Ball ball = col.GetComponent<Ball>();
        if (ball)
            GameManager.Goal(team);
    }
    #endregion


}
