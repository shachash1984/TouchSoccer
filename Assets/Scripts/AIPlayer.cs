using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum GameState { Play, StandBy}
public class AIPlayer : MonoBehaviour {

    #region Fields
    private int _maxLevel = 5;
    private Team _team;
    [Range(1, 5)]
    [SerializeField] private int _level = 1;
    private LocalPlayer _player;
    private Vector3 _shotDirection;
    [SerializeField] private float _xDirectionBound = 1f;
    [SerializeField] private float _zDirectionBoundMax = -1f;
    [SerializeField] private float _zDirectionBoundMin = -0.1f;
    [SerializeField] private float _delayBeforeCatchingBallMin = 0.5f;
    [SerializeField] private float _delayBeforeCatchingBallMax = 1f;
    [SerializeField] private float _delayBeforeShootingBallMin = 2f;
    [SerializeField] private float _delayBeforeShootingBallMax = 3f;
    [SerializeField] private float _shotForceMin = 1f;
    [SerializeField] private float _shotForceMax = 2f;
    [SerializeField] private GameState _gameState;
    [SerializeField] private Ball _ball;
    #endregion

    #region MonoBehaviour Callbacks
    void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        GameManager.OnWin += GameManager_OnWin;
        GameManager.OnGameStarted += GameManager_OnGameStarted;
    }

    private void OnDisable()
    {
        GameManager.OnWin -= GameManager_OnWin;
        GameManager.OnGameStarted -= GameManager_OnGameStarted;
    }


    #endregion

    #region GameManager Event Handlers
    private void GameManager_OnWin(Team team = Team.Null)
    {
        _gameState = GameState.StandBy;
    }

    private void GameManager_OnGameStarted(Team team = Team.Null)
    {
        _gameState = GameState.Play;
        StartCoroutine(Play());
    }
    #endregion

    #region Methods
    public void Init()
    {
        _team = Team.Red;
        GameManager.GameStarted(_team);
    }

    IEnumerator Play()
    {
        while (_gameState == GameState.Play)
        {
            yield return new WaitForSeconds(Random.Range(_delayBeforeCatchingBallMin, _delayBeforeCatchingBallMax));
            CatchBall:
            if (_ball.GetZone() == Zone.Enemy)
            {
                _ball.Catch(true);
            }
            float randX = Random.Range(-_xDirectionBound, _xDirectionBound);
            float randZ = Random.Range(_zDirectionBoundMin, _zDirectionBoundMax);
            yield return new WaitForSeconds(Random.Range(_delayBeforeShootingBallMin, _delayBeforeShootingBallMax));
            if (_ball.isCaught && _ball.GetZone() == Zone.Enemy)
            {
                SetShotDirection(new Vector3(randX, 0f, randZ));
                float randomForce = Random.Range(_shotForceMin, _shotForceMax);
                _ball.Flick(_shotDirection, randomForce, false);
            }
            else
                goto CatchBall;
            
        }
    }

    void SetShotDirection(Vector3 wantedDir)
    {
        _shotDirection = wantedDir;
    }

    /*void MoveOffense()
    {
        float xPos = Random.Range(-_offenseXBound, _offenseXBound);
        float zPos = Random.Range(_offenseZBoundMin, _offenseZBoundMax);
        Vector3 temp = new Vector3(xPos, _offensePlayer.transform.position.y, zPos);
        _offensePlayer.transform.DOMove(temp, 0.5f);
    }*/

    /*void MoveDefense()
    {
        float xPos = Random.Range(-_defenseXBound, _defenseXBound);
        _defensePlayer.transform.DOMove(new Vector3(xPos, _defensePlayer.transform.position.y, _defensePlayer.transform.position.z), 0.5f);
    }*/

    /*private void AdjustRandomRangeToLevel()
    {
        float _forceErrorRange = 0f;
        float _directionErrorRange = 0f;
        switch (_level)
        {
            case 1:
                _forceErrorRange = 0f;
                _directionErrorRange = 0f;
                break;
            case 2:
                _forceErrorRange = 0.1f;
                _directionErrorRange = 0.02f;
                break;
            case 3:
                _forceErrorRange = 0.2f;
                _directionErrorRange = 0.04f;
                break;
            case 4:
                _forceErrorRange = 0.3f;
                _directionErrorRange = 0.06f;
                break;
            case 5:
                _forceErrorRange = 0.4f;
                _directionErrorRange = 0.08f;
                break;
            default:
                Debug.LogError("Parameters are not set to level " + _level);
                break;
        }
        _forceMultMin += _forceErrorRange;
        _forceMultMax -= _forceErrorRange;
        _directionFactorMin += _directionErrorRange;
        _directionFactorMax -= _directionErrorRange;
    }*/

    
    #endregion

}
