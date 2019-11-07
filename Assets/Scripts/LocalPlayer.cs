using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TouchScript.Gestures;

public class LocalPlayer : MonoBehaviour {

    #region Fields
    static public LocalPlayer S;
    private SoccerPlayer _touchedSoccerPlayer;
    private SoccerPlayer _offensePlayer;
    private SoccerPlayer _defensePlayer;
    public Team team;
    public Arrow shotArrow;
    public SpriteRenderer shotArrowRenderer;
    [SerializeField] protected float _shotForce;
    private Vector3 _shotDirection;
    private Camera _camera;
    private GameState _gameState = GameState.StandBy;
    private Dictionary<int, SoccerPlayer> _currentSoccerPlayers = new Dictionary<int, SoccerPlayer>();
    private float _aimingAreaLimit = 2.5f;
    private Ray _ray;
    private RaycastHit _hit;
    [SerializeField] private Ball _ball;
    [SerializeField] private MetaGesture _ballMGesture;
    private float _distanceBetweenTouches = 0f;
    [SerializeField] private float _flickForceMult = 5;
    private Vector3 _directionBetweenTouches = Vector3.zero;

    #endregion

    #region Monobehaviour Callbacks
    void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
        Init();
    }

    private void FixedUpdate()
    {
        if(Input.touchCount > 0 && _ball.GetZone() == Zone.Player)
        {
            MoveBall();
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameStarted += GameManager_OnGameStarted;
        GameManager.OnWin += GameManager_OnWin;
        _ballMGesture.StateChanged += _ballMGesture_StateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStarted -= GameManager_OnGameStarted;
        GameManager.OnWin -= GameManager_OnWin;
        _ballMGesture.StateChanged -= _ballMGesture_StateChanged;
    }

    #endregion

    #region Event Handlers
    private void _ballMGesture_StateChanged(object sender, GestureStateChangeEventArgs e)
    {
        //Debug.Log(_ballMGesture.State);
        
        switch (_ballMGesture.State)
        {
            case Gesture.GestureState.Idle:
                break;
            case Gesture.GestureState.Possible:
                break;
            case Gesture.GestureState.Began:
                break;
            case Gesture.GestureState.Changed:
                _distanceBetweenTouches = Input.GetTouch(0).deltaPosition.magnitude;//Vector3.Distance(_ballMGesture.NormalizedScreenPosition, _ballMGesture.PreviousNormalizedScreenPosition);
                
                _directionBetweenTouches = Input.GetTouch(0).deltaPosition;//(_ballMGesture.NormalizedScreenPosition - _ballMGesture.PreviousNormalizedScreenPosition);
                _directionBetweenTouches.z = _directionBetweenTouches.y;
                _directionBetweenTouches.y = 0f;
                break;
            case Gesture.GestureState.Ended:
            case Gesture.GestureState.Cancelled:
            case Gesture.GestureState.Failed:
                _ball.Flick(_directionBetweenTouches, _distanceBetweenTouches * _flickForceMult);
                _directionBetweenTouches = Vector3.zero;
                _distanceBetweenTouches = 0f;
                break;
            default:
                break;
        }
    }

    private void GameManager_OnGameStarted(Team team = Team.Null)
    {
        _gameState = GameState.Play;
    }

    private void GameManager_OnWin(Team team = Team.Null)
    {
        _gameState = GameState.StandBy;
    }

    #endregion

    #region Methods
    /// <summary>
    /// Initialize
    /// </summary>
    public void Init()
    {
        //ToggleArrow(false, true);
        if (!_camera)
            _camera = Camera.main;
        SetTeam(Team.Blue);
        SetCamera(team);
    }

    /// <summary>
    /// Control the movement of the ball in your half of the court
    /// </summary>
    public void MoveBall()
    {
        if (!_ball.isCaught && _ball.GetZone() == Zone.Player)
        {
            _ray = _camera.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(_ray, out _hit))
            {
                if (_hit.collider.gameObject.layer == 10)
                {
                    _ball.Catch(true);
                }
            }
        }
        else if (_ball.isCaught)
        {
            Vector3 wantedPos = _camera.ScreenToWorldPoint(Input.GetTouch(0).position);
            if (wantedPos.z > -4.5f && wantedPos.z < 0f)
            {
                wantedPos.y = _ball.transform.position.y;
                _ball.transform.position = wantedPos;
            }
        }
    }

    /*public void ManageSoccerPlayerByTouch()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            _ray = _camera.ScreenPointToRay(Input.GetTouch(i).position);
            if (Physics.Raycast(_ray, out _hit))
            {
                if (_hit.collider.gameObject.layer == 11)
                {
                    _touchedSoccerPlayer = _hit.collider.GetComponent<SoccerPlayer>();
                    switch (Input.touches[i].phase)
                    {
                        case TouchPhase.Began:
                            if (!_touchedSoccerPlayer.selected)
                                SelectSoccerPlayer(Input.GetTouch(i).fingerId, _touchedSoccerPlayer);
                            break;
                        case TouchPhase.Moved:
                            if (_touchedSoccerPlayer.GetState() is MovingState)
                                _touchedSoccerPlayer.SetStationaryBar(0f, 0.01f);
                            break;
                        case TouchPhase.Stationary:
                            if(_touchedSoccerPlayer.GetState() is MovingState)
                            {
                                if (Mathf.Abs(_touchedSoccerPlayer.transform.position.z) <= _aimingAreaLimit)
                                {
                                    _touchedSoccerPlayer.SetStationaryBar(0.01f);
                                    if (_touchedSoccerPlayer.GetStationaryBar() >= 1)
                                    {
                                        _offensePlayer = _touchedSoccerPlayer;
                                        _offensePlayer.SetState(_offensePlayer.gameObject.AddComponent<AimingState>());
                                    }
                                }
                            }
                            break;
                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            if (_touchedSoccerPlayer.selected)
                                ReleaseSoccerPlayer(_touchedSoccerPlayer);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (Input.touches[i].phase)
                    {
                        case TouchPhase.Began:
                            break;
                        case TouchPhase.Moved:
                            break;
                        case TouchPhase.Stationary:
                            break;
                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                        if (_offensePlayer && _offensePlayer.GetState() is AimingState)
                                ReleaseSoccerPlayer(_offensePlayer);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }*/

    /// <summary>
    /// Select a Disc by touching it
    /// </summary>
    /*public void SelectSoccerPlayer(int id, SoccerPlayer sp)
    {
        sp.id = id;
        sp.selected = true;
        AddSoccerPlayer(sp.id, sp);
        if (_currentSoccerPlayers[sp.id].GetState() is IdleState)
            _currentSoccerPlayers[sp.id].SetState(_currentSoccerPlayers[sp.id].gameObject.AddComponent<MovingState>());
        sp.InitBall();
    }*/

    /// <summary>
    /// Release current Disc
    /// </summary>
    /*public void ReleaseSoccerPlayer(SoccerPlayer sp)
    {
        RemoveSoccerPlayer(sp.id);
        sp.id = -2;
        sp.SetStationaryBar(0f, 1f);
        sp.SetState(sp.gameObject.AddComponent<IdleState>());
        sp.selected = false; 
    }*/

    /// <summary>
    /// Toggle the arrow GameObject
    /// </summary>
    /// <param name="show"> show = activate</param>
    /// <param name="immediate"> activate/deactivate without fade</param>
    /*private void ToggleArrow(bool show, bool immediate = false)
    {
        if (immediate)
        {
            if (show)
                shotArrowRenderer.DOFade(1f, 0f);
            else
                shotArrowRenderer.DOFade(0f, 0f);
        }
        else
        {
            if (show)
                shotArrowRenderer.DOFade(1f, 0.1f);
            else
                shotArrowRenderer.DOFade(0f, 0.1f);
        }
    }*/

    public void SetTeam(Team t)
    {
        team = t;
    }

    private void SetCamera(Team t)
    {
        if (t == Team.Blue)
            _camera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        else
            _camera.transform.rotation = Quaternion.Euler(90f, 0f, 180f);
    }

    /*public void AddSoccerPlayer(int id ,SoccerPlayer d)
    {
        if (!ContainsSoccerPlayer(id))
            _currentSoccerPlayers.Add(id, d);
    }*/

    /*public void RemoveSoccerPlayer(int id)
    {
        _currentSoccerPlayers.Remove(id);
    }*/

    /*public bool ContainsSoccerPlayer(int id)
    {
        return _currentSoccerPlayers.ContainsKey(id);
    }*/

    /*public SoccerPlayer GetSoccerPlayer(int id)
    {
        return _currentSoccerPlayers[id];
    }*/
    #endregion

}
