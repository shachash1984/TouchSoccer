using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public enum SoccerPlayerType { Defense, Offense}
public class SoccerPlayer : MonoBehaviour {


    #region Fields
    public Team team;
    public SoccerPlayerType soccerPlayerType;
    public Vector2 lastTouchPosition;
    public Vector3 initialPosition;
    private Vector3 _shotDirection;
    [SerializeField] float _shotForce = 5f;
    private Rigidbody _ballRigidbody;
    public float resetPositionDelay = 0.75f;
    public int id = -1;
    public bool selected = false;
    [SerializeField] private State _state;
    [SerializeField] private Ball _ball;
    private Vector3 _ballInitialLocalPos = new Vector3(0f, 0.4f, 0.5f);
    private Camera _camera;
    private Arrow _shotArrow;
    private SpriteRenderer _shotArrowRenderer;
    [SerializeField] private Image _stationaryBar;
    #endregion

    #region MonoBehaviour Callbacks
    void Awake()
    {
        Init();
    }

    private void Update()
    {
        _state.Action();
    }

    void OnEnable()
    {
        GameManager.OnGoal += GameManager_OnGoal;
        GameManager.OnSelfGoal += GameManager_OnSelfGoal;
    }

    void OnDisable()
    {
        GameManager.OnGoal -= GameManager_OnGoal;
        GameManager.OnSelfGoal -= GameManager_OnSelfGoal;
    }

    #endregion

    #region GameManager Event Handlers
    private void GameManager_OnGoal(Team team = Team.Null)
    {
        if (team != this.team)
            Init();
    }

    private void GameManager_OnSelfGoal(Team team = Team.Null)
    {
        if (team == this.team)
            Init();
    }
    #endregion

    #region Methods
    public void Shoot()
    {
        if (_ball)
        {
            _ball.transform.parent = null;
            _ball.GetComponent<Rigidbody>().isKinematic = false;
        }
            
        _ballRigidbody.AddForce(_shotDirection.normalized * _shotForce, ForceMode.Impulse);
    }

    public void Shoot(Vector3 dir, float force)
    {
        if (_ball)
        {
            _ball.transform.parent = null;
            _ball.GetComponent<Rigidbody>().isKinematic = false;
        }
            
        _ballRigidbody.AddForce(dir.normalized * force, ForceMode.Impulse);
    }

    public void Init()
    {
        if (!_camera)
            _camera = Camera.main;
        if (!_shotArrow)
        {
            _shotArrow = FindObjectOfType<Arrow>();
            _shotArrowRenderer = _shotArrow.GetComponent<SpriteRenderer>();
        }
        if (!(_state is IdleState))
            SetState(gameObject.AddComponent<IdleState>());
        SetStationaryBar(0f, 1f);
        //Renderer r = GetComponent<Renderer>();
        //r.material.DOFade(0f, 0f);
        if (!_ballRigidbody)
            _ballRigidbody = _ball.GetComponent<Rigidbody>();
        _ballRigidbody.isKinematic = true;
        transform.position = initialPosition;
        selected = false;
        //Stop();
        _ballRigidbody.isKinematic = false;
        //r.material.DOFade(1f, 0.5f);
    }

    /*public void Stop()
    {
        _ballRigidbody.velocity = Vector3.zero;
        _ballRigidbody.angularVelocity = Vector3.zero;
    }*/

    /// <summary>
    /// Toggle the arrow GameObject
    /// </summary>
    /// <param name="show"> show = activate</param>
    /// <param name="immediate"> activate/deactivate without fade</param>
    public void ToggleArrow(bool show, bool immediate = false)
    {
        if (immediate)
        {
            if (show)
                _shotArrowRenderer.DOFade(1f, 0f);
            else
                _shotArrowRenderer.DOFade(0f, 0f);
        }
        else
        {
            if (show)
                _shotArrowRenderer.DOFade(1f, 0.1f);
            else
                _shotArrowRenderer.DOFade(0f, 0.1f);
        }
    }

    public void Move()
    {
        Vector3 wantedPos = Vector3.zero;
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (id == Input.touches[i].fingerId)
            {
                wantedPos = _camera.ScreenToWorldPoint(Input.touches[i].position);
                break;
            }
                
        }
        wantedPos.y = transform.position.y;
        if (IsTouchInBounds(wantedPos))
            transform.position = wantedPos;
        else
            return;
    }

    public IEnumerator Aim()
    {
        Ray shotRay;
        RaycastHit shotHit;
        Vector3 touchPos = Vector3.zero;
        _shotArrow.Init(transform);
        ToggleArrow(true);
        while (_state is AimingState)
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < Input.touchCount; i++)
            {
                if(Input.touches[i].fingerId == id)
                {
                    shotRay = _camera.ScreenPointToRay(Input.touches[i].position);
                    if (Physics.Raycast(shotRay, out shotHit))
                    {
                        touchPos = shotHit.point;
                        _shotDirection = (transform.position - touchPos);
                        _shotDirection.y = 0f;
                        _shotArrow.SetDirection(transform.position + _shotDirection);
                    }
                }
            }
        }
    }

    public void SetState(State newState)
    {
        if (_state)
            _state.OnExitState();

        _state = newState;
        _state.OnEnterState();

    }

    public State GetState()
    {
        return _state;
    }

    public bool IsTouchInBounds(Vector3 touchPos)
    {
        if (team == Team.Blue)
            return touchPos.z < 0 && Mathf.Abs(touchPos.x) < 2.5f;
        else if (team == Team.Red)
            return touchPos.z > 0 && Mathf.Abs(touchPos.x) < 2.5f;
        else
            return false;
    }


    public void SetStationaryBar(float increase, float decrease = 0)
    {
        _stationaryBar.fillAmount += increase;
        _stationaryBar.fillAmount -= decrease;
    }

    public float GetStationaryBar()
    {
        return _stationaryBar.fillAmount;
    }

    public void InitBall()
    {
        if (soccerPlayerType == SoccerPlayerType.Offense && _ball)
        {
            _ball.Stop();
            _ball.transform.parent = transform;
            _ball.transform.localPosition = _ballInitialLocalPos;
            _ball.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    #endregion


}
