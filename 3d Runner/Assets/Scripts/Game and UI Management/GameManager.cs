using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    [Header("Game State")]
    public bool IsRunning;
    public bool PlayerDied;

    [Header("Scripts")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private TerrainController _terrainController;
    [SerializeField] private CopController _copController;

    [Header("Other GOs")]
    [SerializeField] private GameObject _terrainGO;

    [Header("Other Values")]
    [SerializeField] private float _deathTime = 3f;

    [Header("Camera")]
    [SerializeField] private GameObject _cameraGO;
    [SerializeField] private CameraController _cam;

    [Header("Score Management")]
    [SerializeField] private float _scoreIncrease = 0.0003f;
    [SerializeField] private float _scoreMultiplier = 0.005f;
    [HideInInspector] public float gameScore;
    [HideInInspector] public int coins;

    private float _currentDeathTime;

    // Managers
    private UIController _UIController;
    private PowerUpManager _PUManager;

    // Start is called before the first frame update
    void Start()
    {
        PlayerDied = false;
        _currentDeathTime = _deathTime;
        _UIController = GetComponent<UIController>();
        _PUManager = GetComponent<PowerUpManager>();
        coins = 0;
        gameScore = 0;
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if (IsRunning)
        {
            HandleGameScore();
        }
        DeathTimerHandler();
    }


    public void ActivateMagnet()
    {
        _PUManager.ActivateMagnet();
    }

    public void AddCoin()
    {
        _UIController.SetCoinImageLarge();
        coins += 1;
    }

    public void DecreaseDeathTime()
    {
        _currentDeathTime -= _deathTime / 5;
    }

    private void DeathTimerHandler()
    {
        if (!PlayerDied) return;
        if (_currentDeathTime <= 0)
        {
            RestartGame();
        }
        _currentDeathTime -= Time.deltaTime;
        _UIController.SetDeathTimer(_currentDeathTime / _deathTime);
    }

    private void HandleGameScore()
    {
        gameScore += _scoreMultiplier * Time.deltaTime;
        _scoreMultiplier += _scoreIncrease;
        int _intValue = (int)gameScore;
        _UIController.SetScoreText(_intValue.ToString("000000"));
        _UIController.SetCoinsText(coins.ToString());
    }

    public void StartGame()
    {
        _UIController.SetStartMenu(false);
        _UIController.SetPlayMenu(true);
        IsRunning = true;
        _playerController.StartRunning();
        _cam.SetCameraPos();
        _terrainController.StartMoving();
        _copController.StartMoving();
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void OnPlayerDied()
    {
        PlayerDied = true;
        IsRunning = false;
        _UIController.SetDeathMenu(true);
        _terrainController.StopMoving();
        _currentDeathTime = _deathTime;
    }

}
