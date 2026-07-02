using UnityEngine;

public enum GameState
{
    Ready,
    Playing,
    Pause,
    Win,
    Lose
}


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject WinPanel;
    [SerializeField] GameObject LosePanel;

    public GameState currentState { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        currentState = GameState.Playing;
        WinPanel.SetActive(false);
        LosePanel.SetActive(false);
    }

    /// <summary>
    /// èüóòèàóù
    /// </summary>
    public void Win()
    {
        currentState = GameState.Win;
        WinPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// îsñkèàóù
    /// </summary>
    public void Lose()
    {
        currentState=GameState.Lose;
        LosePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;

        FadeManager.instance.FadeToScene("GameScene");
    }

    public void BackTitle()
    {
        Time.timeScale = 1f;
        FadeManager.instance.FadeToScene("TitleScene");
    }
}
