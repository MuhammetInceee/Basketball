using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameState gameState;
    private float _timer = 50;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject inGameCanvas;
    [SerializeField] private GameObject levelEndCanvas;

    private void Awake()
    {
        gameState = GameState.Start;
    }

    private void Update()
    {
        if(gameState != GameState.Game) return;
        TimeCounter();
    }

    public void GameStateChanger()
    {
        gameState = GameState.Game;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void TimeCounter()
    {
        _timer -= Time.deltaTime;
        timerText.text = $"Time : {(int)_timer}";
        if (_timer <= 0)
        {
            LevelEnd();
        }
    }

    private void LevelEnd()
    {
        gameState = GameState.End;
        inGameCanvas.SetActive(false);
        levelEndCanvas.SetActive(true);
    }

    internal void ScoreUpdate(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}
public enum GameState { Start, Game, End }