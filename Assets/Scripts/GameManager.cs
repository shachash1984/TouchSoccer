using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Team { Null = 2, Blue = 0, Red = 1}

public class GameManager : MonoBehaviour {

    #region Fields
    static public GameManager S;
    public delegate void GameAction(Team team = Team.Null);
    public static event GameAction OnGameStarted;
    public static event GameAction OnGoal;
    public static event GameAction OnSelfGoal;
    public static event GameAction OnWin;
    public SoccerPlayer[] discs;
    public int blueScore = 0;
    public int redScore = 0;
    public int discsPerTeam = 3;
    #endregion

    #region MonoBehaviour Callbacks
    void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
        Init();
    }

    void OnEnable()
    {
        OnGoal += GameManager_OnGoal;
        OnSelfGoal += GameManager_OnSelfGoal;
    }

    void OnDisable()
    {
        OnGoal -= GameManager_OnGoal;
        OnSelfGoal -= GameManager_OnSelfGoal;
    }
    #endregion

    #region EventTriggers
    public static void Goal(Team team)
    {
        if (OnGoal != null)
            OnGoal(team);
    }

    public static void SelfGoal(Team team)
    {
        if (OnSelfGoal != null)
            OnSelfGoal(team);
    }

    public static void Win(Team team)
    {
        if (OnWin != null)
            OnWin(team);
    }

    public static void GameStarted(Team team)
    {
        if (OnGameStarted != null)
            OnGameStarted(team);
    }

    #endregion

    #region EventHandlers
    private void GameManager_OnGoal(Team team = Team.Null)
    {
        UpdateScore(team);
    }

    private void GameManager_OnSelfGoal(Team team = Team.Null)
    {
        UpdateScore(team);
    }

    #endregion

    #region Methods
    private void UpdateScore(Team team)
    {
        if (team == Team.Red)
            blueScore++;
        else
            redScore++;

        if (blueScore >= 2 || redScore >= 2)
            Win(team);
    }

    public void Init()
    {
        redScore = 0;
        blueScore = 0;

    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    #endregion

}
