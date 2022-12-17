using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public bool IsRunning = false;
    public bool _playerDied;
    PlayerController _playerController;
    TerrainController _terrainController;

    [Header("Other GOs")]
    [SerializeField] private GameObject _terrainGO;

    [Header("Other Values")]
    [SerializeField] private float _deathTime = 3f;

    [Header("Camera")]
    [SerializeField] private GameObject _cameraGO;
    CameraController _cam;

    [Header("UI Elements")]
    [SerializeField] private GameObject _startMenu;
    [SerializeField] private GameObject _deathMenu;
    [SerializeField] private GameObject _playMenu;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private RectTransform _coinImage;
    [SerializeField] private Image _deathTimer;

    [Header("Score Management")]
    [SerializeField] private float _scoreIncrease = 0.0003f;
    [SerializeField] private float _scoreMultiplier = 0.005f;
    [HideInInspector] public float gameScore;
    [HideInInspector] public int coins;

    [Header("Coin Image")]
    [SerializeField] private float _coinSize = 56.358f;
    [SerializeField] private float _largeCoinSize = 70f;
    private float _currentCoinSize;

    private float _currentDeathTime;

    // Start is called before the first frame update
    void Start()
    {
        _playerDied = false;
        _currentDeathTime = _deathTime;
        _currentCoinSize = _coinSize;
        coins = 0;
        gameScore = 0;
        Application.targetFrameRate = 60;
        _terrainController = _terrainGO.GetComponent<TerrainController>();
        _cam = _cameraGO.GetComponent<CameraController>();
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (IsRunning)
        {
            HandleGameScore();
        }
        HandleCoinImageAnimation();
        DeathTimerHandler();
    }

    private void HandleCoinImageAnimation()
    {
        _currentCoinSize = Mathf.Lerp(_currentCoinSize, _coinSize, 4 * Time.deltaTime);
        _coinImage.sizeDelta = new Vector2(_currentCoinSize, _currentCoinSize);
    }

    public void AddCoin()
    {
        _currentCoinSize = _largeCoinSize;
        coins += 1;
    }

    public void DecreaseDeathTime()
    {
        _currentDeathTime -= _deathTime / 5;
    }

    private void DeathTimerHandler()
    {
        if (!_playerDied) return;
        if (_currentDeathTime <= 0)
        {
            RestartGame();
        }
        _currentDeathTime -= Time.deltaTime;
        _deathTimer.fillAmount = _currentDeathTime / _deathTime;
    }

    private void HandleGameScore()
    {
        gameScore += _scoreMultiplier * Time.deltaTime;
        _scoreMultiplier += _scoreIncrease;
        int _intValue = (int)gameScore;
        _scoreText.SetText(_intValue.ToString("000000"));
        _coinsText.SetText(coins.ToString());
    }

    public void StartGame()
    {
        _startMenu.SetActive(false);
        _playMenu.SetActive(true);
        IsRunning = true;
        _playerController.StartRunning();
        _cam.SetCameraPos();
        _terrainController.StartMoving();
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void OnPlayerDied()
    {
        _playerDied = true;
        IsRunning = false;
        _deathMenu.SetActive(true);
        _terrainController.StopMoving();
        _currentDeathTime = _deathTime;
    }

}
