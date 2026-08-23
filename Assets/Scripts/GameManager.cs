using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    [Header("UI")]
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject Menu;
    [SerializeField] private GameObject gamePause;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject gameWon;

    private Coroutine subscribeRoutine;

    private void OnEnable()
    {
        subscribeRoutine = StartCoroutine(SubscribeWhenReady());
    }

    private void OnDisable()
    {
        if (subscribeRoutine != null)
        {
            StopCoroutine(subscribeRoutine);
            subscribeRoutine = null;
            GridManager.Instance.playerBase.OnBuildingDestroyed -= GameOver;
        }
    }

    // Makes sure there is a player base before subscribing/
    private IEnumerator SubscribeWhenReady()
    {
        yield return new WaitUntil(() => GridManager.Instance != null && GridManager.Instance.playerBase != null && EnemyManager.Instance != null);

        GridManager.Instance.playerBase.OnBuildingDestroyed += GameOver;
        EnemyManager.Instance.OnEnemyKilled += (enemy) => CheckVictory();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HUD.SetActive(true);
        Menu.SetActive(false);
        gameOver.SetActive(false);
        gameWon.SetActive(false);
        gamePause.SetActive(false);
        Time.timeScale = 1;

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Pause();
        }
    }
    
    public void Resume()
    {
        HUD.SetActive(true);
        Menu.SetActive(false);
        gamePause.SetActive(false);
        Time.timeScale = 1;
    }

    public void Pause()
    {
        HUD.SetActive(false);
        Menu.SetActive(true);
        gamePause.SetActive(true);
        Time.timeScale = 0;
    }
    
    public void MainMenu()
    {
        // need to add game mod selection menu
        SceneManager.LoadScene(0);
    }
    
    public void Restart()
    {
        // need to add game mod selection menu
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    private void GameOver()
    {
        HUD.SetActive(false);
        Menu.SetActive(true);
        gameOver.SetActive(true);
        gameWon.SetActive(false);
        gamePause.SetActive(false);
        Time.timeScale = 0;

    }

    private void GameWon()
    {
        HUD.SetActive(false);
        Menu.SetActive(true);
        gameOver.SetActive(false);
        gameWon.SetActive(true);
        gamePause.SetActive(false);
        Time.timeScale = 0;
    }

    private void CheckVictory()
    {
        if (GridManager.Instance.bossSpawned && EnemyManager.Instance.ActiveEnemies.Count <= 0)
        {
            GameWon();
        }
    }
}
