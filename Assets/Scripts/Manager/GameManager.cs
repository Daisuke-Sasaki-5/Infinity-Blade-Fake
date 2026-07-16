using UnityEngine;
using UnityEngine.Playables;

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

    [Header("èüóòÅEîsñkÉpÉlÉã")]
    [SerializeField] GameObject WinPanel;
    [SerializeField] GameObject LosePanel;

    [Header("TimeLine")]
    [SerializeField] private PlayableDirector director;
    [SerializeField] private PlayableAsset winTimeline;
    [SerializeField] private PlayableAsset loseTimeline;

    [SerializeField] private Player player;

    [SerializeField] private AudioSource bgmsource;
    [SerializeField] private AudioClip bgmClip;

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

        bgmsource.clip = bgmClip;
        bgmsource.loop = true;
        bgmsource.Play();
    }

    /// <summary>
    /// èüóòèàóù
    /// </summary>
    public void Win()
    {
        currentState = GameState.Win;

        player.PlayWinAnimation();

        director.playableAsset = winTimeline;
        director.Play();
    }

    public void ShowWinResult()
    {
        WinPanel.SetActive(true);
        //Time.timeScale = 0f;
    }

    /// <summary>
    /// îsñkèàóù
    /// </summary>
    public void Lose()
    {
        currentState=GameState.Lose;

        director.playableAsset = loseTimeline;
        director.Play();
    }

    public void ShowLoseResult()
    {
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
