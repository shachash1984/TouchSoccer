using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    #region Fields
    [SerializeField] private float fadeDuration = 0.5f;
    [Header("Game UI Elements")]
    [SerializeField] private Text _scoreBlueText;
    [SerializeField] private Text _scoreRedText;
    [SerializeField] private CanvasGroup _gameEndPanel;
    [Space]
    [Header("MainMenu UI Elements")]
    [SerializeField] private CanvasGroup _settingsPanel;
    [SerializeField] private CanvasGroup _scoreBoardPanel;
    [SerializeField] private CanvasGroup _profilePanel;
    [SerializeField] private CanvasGroup _shopPanel;
    [SerializeField] private CanvasGroup _playFriendPanel;
    private GameState _gameState = GameState.StandBy;
    #endregion

    #region MonoBehaviour Callbacks
    void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
            InitGame();
    }

    void OnEnable()
    {
        GameManager.OnGoal += GameManager_OnGoal;
        GameManager.OnSelfGoal += GameManager_OnSelfGoal;
        GameManager.OnWin += GameManager_OnWin;
        GameManager.OnGameStarted += GameManager_OnGameStarted;
    }

    
    void OnDisable()
    {
        GameManager.OnGoal -= GameManager_OnGoal;
        GameManager.OnSelfGoal -= GameManager_OnSelfGoal;
        GameManager.OnWin -= GameManager_OnWin;
        GameManager.OnGameStarted -= GameManager_OnGameStarted;
    }
    #endregion

    #region GameManager Event Handlers
    private void GameManager_OnGoal(Team team = Team.Null)
    {
        if (_gameState == GameState.Play)
            StartCoroutine(SetScore());
    }
    private void GameManager_OnSelfGoal(Team team = Team.Null)
    {
        if (_gameState == GameState.Play)
            StartCoroutine(SetScore());
    }
    private void GameManager_OnWin(Team team = Team.Null)
    {
        _gameState = GameState.StandBy;
        StartCoroutine(ToggleElement(_gameEndPanel, true, 1f));
    }
    private void GameManager_OnGameStarted(Team team = Team.Null)
    {
        _gameState = GameState.Play;
    }

    #endregion

    #region Game UI Methods
    void InitGame()
    {
        Input.backButtonLeavesApp = false;
        ToggleElement(_gameEndPanel, false, true);
        StartCoroutine(SetScore());
    }

    IEnumerator SetScore()
    {
        yield return new WaitForEndOfFrame();
        _scoreRedText.text = string.Format("Score: {0}", GameManager.S.redScore);
        _scoreBlueText.text = string.Format("Score: {0}", GameManager.S.blueScore);
    }

    void ToggleElement(CanvasGroup cg, bool on, bool withoutFade = false)
    {
        if (withoutFade)
        {
            if (on)
            {
                cg.blocksRaycasts = true;
                cg.DOFade(1f, 0f);
            }
            else
            {
                cg.blocksRaycasts = false;
                cg.DOFade(0f, 0f);
            }
        }
        else
        {
            if (on)
            {
                cg.blocksRaycasts = true;
                cg.DOFade(1f, fadeDuration);
            }
            else
            {
                cg.blocksRaycasts = false;
                cg.DOFade(0f, fadeDuration);
            }
        }
    }

    IEnumerator ToggleElement(CanvasGroup cg, bool on, float delay, bool withoutFade = false)
    {
        yield return new WaitForSeconds(delay);
        if (withoutFade)
        {
            if (on)
            {
                cg.blocksRaycasts = true;
                cg.DOFade(1f, 0f);
            }
            else
            {
                cg.blocksRaycasts = false;
                cg.DOFade(0f, 0f);
            }
        }
        else
        {
            if (on)
            {
                cg.blocksRaycasts = true;
                cg.DOFade(1f, fadeDuration);
            }
            else
            {
                cg.blocksRaycasts = false;
                cg.DOFade(0f, fadeDuration);
            }
        }
    }
    #endregion

    #region MainMenu UI Methods
    void InitMainMenu()
    {
        Input.backButtonLeavesApp = true;
        ToggleElement(_playFriendPanel, false, true);
        ToggleElement(_shopPanel, false, true);
        ToggleElement(_profilePanel, false, true);
        ToggleElement(_scoreBoardPanel, false, true);
        ToggleElement(_settingsPanel, false, true);
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }
    #endregion


}
