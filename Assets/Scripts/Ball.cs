using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TouchScript.Gestures;

public enum Zone { Player, Enemy}
public class Ball : MonoBehaviour {

    #region Fields
    public Team team;
    [SerializeField] private Zone _zone;
    [SerializeField] private GameObject _halo;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private MetaGesture _mGesture;
    public Vector3 initialPositionPlayer;
    public Vector3 initialPositionEnemy;
    public bool isStuck = false;
    public bool isReady = false;
    private int _edgeLayerIndex = 9;
    public bool isCaught = false;
    #endregion

    #region MonoBehaviour Callbacks
    void OnEnable()
    {
        GameManager.OnGameStarted += GameManager_OnGameStarted;
        GameManager.OnGoal += GameManager_OnGoal;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= GameManager_OnGameStarted;
        GameManager.OnGoal -= GameManager_OnGoal;
    }

    private void FixedUpdate()
    {
        if (transform.position.z > 0)
        {
            _zone = Zone.Enemy;
        }
        else
        {
            _zone = Zone.Player;
        }
            
    }
    #endregion

    #region GameManager Event Handlers
    private void GameManager_OnGameStarted(Team team = Team.Null)
    {
        isReady = true;
    }

    private void GameManager_OnGoal(Team team = Team.Null)
    {
        int t = (int)team;
        Init(t);
    }

    #endregion

    #region Methods
    public void Init(int team)
    {
        if (team == 0)
            StartCoroutine(ResetPosition(Team.Blue));
        else
            StartCoroutine(ResetPosition(Team.Red));
    }

    public Zone GetZone()
    {
        return _zone;
    }

    public void ToggleHalo(bool on)
    {
        _halo.SetActive(on);
    }

    public void Flick(Vector3 direction, float force, bool normalize = true)
    {
        if (normalize)
            _rigidbody.AddForce(direction.normalized * force, ForceMode.Impulse);
        else
            _rigidbody.AddForce(direction * force, ForceMode.Impulse);
        Catch(false);
    }

    public void Catch(bool caught)
    {
        if (caught)
            Stop();
        isCaught = caught;
        ToggleHalo(caught);
        
    }

    public void ToggleKinemtaic(bool on)
    {
        _rigidbody.isKinematic = on;
    }

    IEnumerator ResetPosition(Team t)
    {
        isReady = false;
        //yield return new WaitForSeconds(_resetPositionDelay);
        Renderer r = GetComponent<Renderer>();
        r.material.DOFade(0f, 0f);
        
        _rigidbody.isKinematic = true;
        Stop();
        if (t == Team.Blue)
            transform.position = initialPositionPlayer;
        else
            transform.position = initialPositionEnemy;
        _rigidbody.isKinematic = false;
        r.material.DOFade(1f, 0.5f);
        yield return new WaitUntil(() => !DOTween.IsTweening(r));
        isReady = true;
    }

    public void Stop()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }
    #endregion


}
