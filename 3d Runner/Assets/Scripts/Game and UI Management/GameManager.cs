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
    [SerializeField] private SkinChangeController _skinChangeController;

    [Header("Other GOs")]
    [SerializeField] private GameObject _terrainGO;
    [SerializeField] private GameObject _multiplierTimeline;
    [SerializeField] private Image _multiplierBar;
    [SerializeField] private GameObject _reviveSmokeEffect;
    [SerializeField] private GameObject _reviveGlowEffect;
    private LockCameraY _camMaxScript;

    [Header("Other Values")]
    [SerializeField] private float _deathTime = 3f;
    [SerializeField] private SwipeDetection _swipeDetection;

    [Header("Camera")]
    [SerializeField] private GameObject _cameraGO;
    [SerializeField] private CameraController _cam;

    [Header("Score Management")]
    [SerializeField] private float _scoreMultiplierIncrease = 0.1f;
    [SerializeField] private float _scoreMultiplier = 0.005f;
    [SerializeField] private float _energyDecrease = 0.005f;
    [HideInInspector] public float gameScore;
    [HideInInspector] public int coins;
    [HideInInspector] public float EnergyLevel;
    public int EnergyItemRate;
    [SerializeField] private float _multiplierBaseDuration;
    [SerializeField] private float _multiplierAddDuration;
    [HideInInspector] public int EnergyItemCount;
    private bool _lerpingEnergyLevel;

    public int PlayerMultiplier;

    public int ItemMultiplier = 1;
    private float _itemMultiplierTimer;

    private float _currentDeathTime;

    [Header("Sound Effects")]
    [SerializeField] private float _homeMusicVolume;
    [SerializeField] private float _playMusicVolume;
    [SerializeField] private float _runningVolume;
    [SerializeField] private AudioClip _buttonSound;
    [SerializeField] private AudioClip _coinPickup;
    [SerializeField] private AudioClip _powerupPickup;
    [SerializeField] private AudioClip _powerupRunOutEffect;

    private AudioSource _runningSource;

    // Managers
    private UIController _UIController;
    private PowerUpManager _PUManager;

    // Start is called before the first frame update
    void Start()
    {
        _skinChangeController.ActivateSelectedSkin();
        Time.timeScale = 1;
        SoundManager.Instance.SetMusicVolume(_homeMusicVolume);
        EnergyItemCount = EnergyItemRate;
        EnergyLevel = 1f;
        _runningSource = GameObject.FindGameObjectWithTag("RunningSource").GetComponent<AudioSource>();
        _runningSource.volume = 0;
        PlayerDied = false;
        _currentDeathTime = _deathTime;
        _UIController = GetComponent<UIController>();
        _PUManager = GetComponent<PowerUpManager>();
        coins = 0;
        gameScore = 0;
        Application.targetFrameRate = 60;
        if (SceneVariables.StartGameOnMainLoad)
        {
            SceneVariables.StartGameOnMainLoad = false;
            StartCoroutine(StartDelayed());
        }
        InitiatePrefs();
        PlayerMultiplier = PlayerPrefs.GetInt("_playerMultiplier");
        _camMaxScript = GameObject.FindGameObjectWithTag("VCam").GetComponent<LockCameraY>();
        _camMaxScript.enabled = false;
    }

    private void InitiatePrefs()
    {
        if (!PlayerPrefs.HasKey("_playerCoins"))
        {
            PlayerPrefs.SetInt("_playerCoins", 0);
        }
        if (!PlayerPrefs.HasKey("_playerDiamonds"))
        {
            PlayerPrefs.SetInt("_playerDiamonds", 0);
        }
        if (!PlayerPrefs.HasKey("_playerMultiplier"))
        {
            PlayerPrefs.SetInt("_playerMultiplier", 2);
        }
        if (!PlayerPrefs.HasKey("_playerHighscore"))
        {
            PlayerPrefs.SetInt("_playerHighscore", 0);
        }
        if (!PlayerPrefs.HasKey("_magnetDuration"))
        {
            PlayerPrefs.SetInt("_magnetDuration", 1);
        }
        if (!PlayerPrefs.HasKey("_multiplierDuration"))
        {
            PlayerPrefs.SetInt("_multiplierDuration", 1);
        }
        if (!PlayerPrefs.HasKey("_selectedSkin"))
        {
            PlayerPrefs.SetInt("_selectedSkin", 0);
        }
        InitializeSkins();
    }

    private void InitializeSkins()
    {
        for (int i = 0; i < _skinChangeController._skins.Count - 1; i++)
        {
            string key = "_skin_" + i.ToString();
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 0);
            }
            PlayerPrefs.SetInt("_skin_0", 1);
        }
        
    }

    public void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }

    public void ShowShopMenu()
    {
        _UIController.SetStartMenu(false);
        _UIController.SetShopMenu(true);
    }

    public void HideShopMenu()
    {
        _UIController.SetStartMenu(true);
        _UIController.SetShopMenu(false);
    }

    public void PlayButtonSound()
    {
        SoundManager.Instance.PlaySound(_buttonSound);
    }

    private IEnumerator StartDelayed()
    {
        _UIController.SetStartMenu(false);
        yield return new WaitForSeconds(.01f);
        StartGame();
    }

    private void Update()
    {
        if (IsRunning)
        {
            HandleGameScore();
        }
        DeathTimerHandler();
        EnergyLevelHandler();
        HandleItemMultiplier();
    }

    private IEnumerator EnergyLerp(float addValue, float duration)
    {
        float timeElapsed = 0;
        _lerpingEnergyLevel = true;
        float origValue = EnergyLevel;
        float aimValue = Mathf.Clamp(addValue + EnergyLevel, 0, 1);
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            EnergyLevel = Mathf.Lerp(origValue, aimValue, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        EnergyLevel = aimValue;
        _lerpingEnergyLevel = false;
    }

    private void EnergyLevelHandler()
    {
        if (IsRunning && !_lerpingEnergyLevel && EnergyLevel > 0)
        {
            EnergyLevel -= _energyDecrease;
        }
        if (IsRunning && _playerController.m_char.isGrounded && EnergyLevel <= 0)
        {
            _playerController.OnEnergyDead();
            StartCoroutine(_UIController.TransitionVignette(0.5f, 1.5f));
        }
        _UIController.SetEnergyBarValue(EnergyLevel);
    }

    public void ActivateMagnet()
    {
        _PUManager.ActivateMagnet();
        SoundManager.Instance.PlaySound(_powerupPickup);
    }

    private IEnumerator DisableReviveSmoke(float delay)
    {
        yield return new WaitForSeconds(delay);
        _reviveSmokeEffect.SetActive(false);
    }

    private IEnumerator DisableReviveGlow(float delay)
    {
        yield return new WaitForSeconds(delay);
        _reviveGlowEffect.GetComponent<Animator>().CrossFadeInFixedTime("item_glow_fade_out", 0.4f);
        yield return new WaitForSeconds(2);
        _reviveGlowEffect.SetActive(false);
    }

    public void Revive()
    {
        _terrainController.OnRevive();
        _UIController.SetDeathMenu(false);
        _UIController.SetPlayMenu(true);
        IsRunning = true;
        PlayerDied = false;
        _playerController.OnRevive();
        _runningSource.volume = _runningVolume;
        EnergyLevel = 1;
        _playerController._newXPos = 0;
        _playerController.m_side = SIDE.Mid;
        StartCoroutine(_UIController.TransitionVignette(0.344f, 0.5f));
        _reviveSmokeEffect.SetActive(true);
        _reviveGlowEffect.SetActive(true);
        StartCoroutine(DisableReviveGlow(0f));
        StartCoroutine(DisableReviveSmoke(4f));
        GetComponent<PowerUpManager>().DeactivateMagnet();
        ItemMultiplier = 1;
        _UIController.DeactivateMultiplierEffect();
        _multiplierTimeline.SetActive(false);
    }

    public void OnExplosion()
    {
        _playerController.OnExplosion();
    }

   
    public void OnEnergyCollect(float energyValue)
    {
        SoundManager.Instance.PlaySound(_powerupPickup);
        _PUManager.ItemCollectEffects();
        StartCoroutine(EnergyLerp(energyValue, 0.15f));
    }

    public void DecreaseEnergy(float energyDecreaseValue)
    {
        StartCoroutine(EnergyLerp(-energyDecreaseValue, 0.15f));
    }

    public void OnDiamondCollect()
    {
        SoundManager.Instance.PlaySound(_powerupPickup);
        _PUManager.OnDiamondCollectEffect();
    }

    public void AddCoin()
    {
        SoundManager.Instance.PlaySound(_coinPickup);
        PlayerPrefs.SetInt("_playerCoins", PlayerPrefs.GetInt("_playerCoins") + 1);
        _UIController.SetCoinImageLarge();
        coins += 1;
    }

    public void AddDiamond()
    {
        PlayerPrefs.SetInt("_playerDiamonds", PlayerPrefs.GetInt("_playerDiamonds") + 1);
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
            GoToGameOverScene();
        }
        _currentDeathTime -= Time.deltaTime;
        _UIController.SetDeathTimer(_currentDeathTime / _deathTime, _currentDeathTime);
    }

    public void ShowSettings()
    {
        _UIController.SetSettingsMenu(true);
        _UIController.SetStartMenu(false);
    }

    public void HideSettingsHome()
    {
        _UIController.SetSettingsMenu(false);
        _UIController.SetStartMenu(true);
    }

    public void PauseGame()
    {
        _swipeDetection.HasSwipedFalse();
        Time.timeScale = 0;
        StartCoroutine(EaseMusicVolume(_homeMusicVolume));
        _UIController.SetPauseMenu(true);
        _runningSource.volume = 0;
    }

    public void ResumeGame()
    {
        _swipeDetection.HasSwipedFalse();
        Time.timeScale = 1;
        StartCoroutine(EaseMusicVolume(_playMusicVolume));
        _UIController.SetPauseMenu(false);
        _runningSource.volume = _runningVolume;
    }

    private void HandleGameScore()
    {
        gameScore += _scoreMultiplier * PlayerMultiplier * ItemMultiplier;
        _scoreMultiplier += _scoreMultiplierIncrease;
        int _intValue = (int)gameScore;
        _UIController.SetScoreText(_intValue.ToString("000000"));
        _UIController.SetCoinsText(coins.ToString());
    }

    public void OnMultiplierItemCollect()
    {
        SoundManager.Instance.PlaySound(_powerupPickup);
        _multiplierTimeline.SetActive(true);
        _PUManager.ItemCollectEffects();
        ItemMultiplier = 2;
        _itemMultiplierTimer = _multiplierBaseDuration + PlayerPrefs.GetInt("_multiplierDuration") * _multiplierAddDuration;
        _UIController.ActivateMultiplierEffect();
    }

    public void GiveCoins(int amount)
    {
        PlayerPrefs.SetInt("_playerCoins", PlayerPrefs.GetInt("_playerCoins") + amount);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }

    private void HandleItemMultiplier()
    {
        if (ItemMultiplier > 1)
        {
            _multiplierBar.fillAmount = _itemMultiplierTimer / (_multiplierBaseDuration + PlayerPrefs.GetInt("_multiplierDuration") * _multiplierAddDuration);
            _itemMultiplierTimer -= Time.deltaTime;
        }
        if (_itemMultiplierTimer <= 0 && ItemMultiplier > 1)
        {
            ItemMultiplier = 1;
            _UIController.DeactivateMultiplierEffect();
            SoundManager.Instance.PlaySound(_powerupRunOutEffect);
            _multiplierTimeline.SetActive(false);
        }
    }

    public void HideChangeSkinMenu()
    {
        _playerController.m_Animator.CrossFadeInFixedTime("Leaning", 2f);
        _UIController.SetSkinChangeMenu(false);
        _skinChangeController.ActivateSelectedSkin();
        StartCoroutine(_UIController.ActivateStartMenuWithAnimation(.4f));
        StartCoroutine(_cam.ChangeToLeanView(1f));
    }

    public void ShowChangeSkinMenu()
    {
        _playerController.m_Animator.CrossFadeInFixedTime("StandIdle", 2f);
        _UIController.SetSkinChangeMenu(true);
        _UIController.SetStartMenu(false);
        StartCoroutine(_cam.ChangeSkinView(1f));
    }

    private IEnumerator EaseMusicVolume(float duration)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            SoundManager.Instance.SetMusicVolume(Mathf.Lerp(_homeMusicVolume, _playMusicVolume, t));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        SoundManager.Instance.SetMusicVolume(_playMusicVolume);
    } 

    public void StartGame()
    {
        _camMaxScript.enabled = true;
        _runningSource.volume = _runningVolume;
        StartCoroutine(_UIController.HideStartMenu());
        StartCoroutine(_UIController.ShowPlayMenu());
        IsRunning = true;
        //_playerController.StartRunning();
        _cam.SetCameraPos();
        _copController.StartMoving();
        StartCoroutine(EaseMusicVolume(0.5f));
    }

    public void Retry()
    {
        SceneVariables.StartGameOnMainLoad = true;
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToGameOverScene()
    {
        Time.timeScale = 1;
        SceneVariables.LastGameScore = Mathf.FloorToInt(gameScore);
        SceneVariables.LastCollectedCoins = coins;
        SceneManager.LoadScene(1);
    }

    public void OnPlayerDied()
    {
        _runningSource.volume = 0;
        PlayerDied = true;
        IsRunning = false;
        _copController.gameObject.SetActive(true);
        //StartCoroutine(_copController.MoveTowardsPlayerDeath(1f));
        _UIController.SetDeathMenu(true);
        _currentDeathTime = _deathTime;
    }

}
