using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject _modal;
    [SerializeField] private TextMeshProUGUI _modalText;
    [SerializeField] private Button _modalButton;
    [SerializeField] private TextMeshProUGUI _modalButtonText;
    [SerializeField] private GameObject _startMenu;
    [SerializeField] private GameObject _deathMenu;
    [SerializeField] private GameObject _playMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _shopMenu;
    [SerializeField] private GameObject _skinChangeMenu;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private RectTransform _coinImage;
    [SerializeField] private TextMeshProUGUI _deathMenuTotalCoins;
    [SerializeField] private Image _deathTimer;
    [SerializeField] private TextMeshProUGUI _deathTimerText;
    [SerializeField] private Image _energyBar;
    [SerializeField] private Image _energySymbol;
    [SerializeField] private GameObject _linesRed;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private TextMeshProUGUI _playMenuMultiplier;
    [SerializeField] private TextMeshProUGUI _playMenuMultiplierX;

    [SerializeField] private Volume _PpVolume;
    [SerializeField] private float _defaultAberation = 0.2f;
    private Vignette _vignette;
    private ChromaticAberration _chromaticAberration;
    private ColorAdjustments _colorAdjustments;


    [Header("Powerup UI")]
    [SerializeField] private GameObject _magnetTimeline;
    [SerializeField] private Image _magnetBar;
    [SerializeField] private GameObject _linesEffect;

    [Header("Coin Image")]
    [SerializeField] private float _coinSize = 56.358f;
    [SerializeField] private float _largeCoinSize = 70f;

    private float _currentCoinSize;
    private Animator _energySymbolAnimator;
    private GameManager _gameManager;


    // Start is called before the first frame update
    void Start()
    {
        _volumeSlider.value = AudioListener.volume;
        _currentCoinSize = _coinSize;
        _energySymbolAnimator = _energySymbol.GetComponent<Animator>();
        _energySymbolAnimator.enabled = false;
        _gameManager = GetComponent<GameManager>();
        _PpVolume.profile.TryGet(out _vignette);
        _PpVolume.profile.TryGet(out _chromaticAberration);
        _PpVolume.profile.TryGet(out _colorAdjustments);
        _playMenuMultiplier.SetText(PlayerPrefs.GetInt("_playerMultiplier").ToString());
    }

    public IEnumerator ChromaticAberation(float intensity, float fallofDuration)
    {
        float timeElapsed = 0;
        while (timeElapsed < fallofDuration)
        {
            float t = timeElapsed / fallofDuration;
            t = t * t * (3f - 2f * t);
            _chromaticAberration.intensity.value = Mathf.Lerp(intensity, _defaultAberation, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void OpenModal(string text, string buttonText, UnityEngine.Events.UnityAction call)
    {
        _modal.SetActive(true);
        _modalText.SetText(text);
        _modalButtonText.SetText(buttonText);
        _modalButton.onClick.AddListener(call);
    }

    public void CloseModal()
    {
        _modal.SetActive(false);
    }

    public IEnumerator TransitionVignette(float newIntensity, float duration)
    {
        float currentIntensity = _vignette.intensity.value;
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            _vignette.intensity.value = Mathf.Lerp(currentIntensity, newIntensity, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void ActivateMultiplierEffect()
    {
        _playMenuMultiplier.SetText((PlayerPrefs.GetInt("_playerMultiplier") * _gameManager.ItemMultiplier).ToString());
        _playMenuMultiplier.GetComponent<Animator>().enabled = true;
        _playMenuMultiplierX.GetComponent<Animator>().enabled = true;
    }

    public void DeactivateMultiplierEffect()
    {
        _playMenuMultiplier.SetText(PlayerPrefs.GetInt("_playerMultiplier").ToString());
        _playMenuMultiplier.GetComponent<Animator>().enabled = false;
        _playMenuMultiplierX.GetComponent<Animator>().enabled = false;
        _playMenuMultiplier.color = new Color(1, 1, 1);
        _playMenuMultiplierX.color = new Color(1, 1, 1);
    }


    // Update is called once per frame
    void Update()
    {
        HandleCoinImageAnimation();
        HandleEnergySymbol();
    }

    public void SetPauseMenu(bool state)
    {
        _pauseMenu.SetActive(state);
        _playMenu.SetActive(!state);
    }

    public void OnAudioChange(float newValue)
    {
        SoundManager.Instance.ChangeMasterVolume(newValue);
    }

    public void SetSkinChangeMenu(bool state)
    {
        _skinChangeMenu.SetActive(state);
    }

    public IEnumerator ActivateStartMenuWithAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetStartMenu(true);
        _startMenu.GetComponent<Animator>().Play("StartMenuAnimateIn");
    }

    public void HandleEnergySymbol()
    {
        if (!_energySymbolAnimator.enabled && _gameManager.EnergyLevel <= 0.33f)
        {
            _energySymbolAnimator.enabled = true;
        }
        else if (_energySymbolAnimator.enabled && _gameManager.EnergyLevel > 0.33f)
        {
            _energySymbolAnimator.enabled = false;
        }
    }

    public void SetEnergyBarValue(float value)
    {
        //_colorAdjustments.saturation.value = value*100 - 100;
        _energyBar.fillAmount = value;
        _energyBar.color = new Color(1, value*1.3f, 0);
        _energySymbol.color = new Color(1, value*1.3f, 0);
    }

    public void SetShopMenu(bool state)
    {
        _shopMenu.SetActive(state);
    }

    public void SetMagnetBarFill(float value)
    {
        _magnetBar.fillAmount = value;
    }

    public void SetLinesEffectState(bool state)
    {
        _linesEffect.SetActive(state);
    }

    public void SetRedLinesEffectState(bool state)
    {
        _linesRed.SetActive(state);
    }

    public void SetMagnetTimeLineState(bool state)
    {
        _magnetTimeline.SetActive(state);
    }

    public IEnumerator HideStartMenu()
    {
        _startMenu.GetComponent<Animator>().Play("StartMenuDisappear");
        yield return new WaitForSeconds(1f);
        SetStartMenu(false);
    }

    public IEnumerator ShowPlayMenu()
    {
        yield return new WaitForSeconds(1.5f);
        SetPlayMenu(true);
    }

    public void SetStartMenu(bool state)
    {
        _startMenu.SetActive(state);
    }

    public void SetPlayMenu(bool state)
    {
        _playMenu.SetActive(state);
    }

    public void SetDeathMenu(bool state)
    {
        StartCoroutine(SetDeathMenuRoutine(state));
    }

    public void SetSettingsMenu(bool state)
    {
        _settingsMenu.SetActive(state);
    }

    private IEnumerator SetDeathMenuRoutine(bool state)
    {
        yield return new WaitForSeconds(.15f);
        _deathMenu.SetActive(state);
        _deathMenuTotalCoins.SetText(PlayerPrefs.GetInt("_playerCoins").ToString());
    }

    public void SetScoreText(string text)
    {
        _scoreText.SetText(text);
    }

    public void SetCoinsText(string text)
    {
        _coinsText.SetText(text);
    }

    public void SetCoinImageSize(float size)
    {
        _currentCoinSize = size;
    }

    public void SetCoinImageLarge()
    {
        _currentCoinSize = _largeCoinSize;
    }

    public void SetDeathTimer(float value, float absoluteTime)
    {
        _deathTimer.fillAmount = value;
        _deathTimerText.SetText(Mathf.CeilToInt(Mathf.Clamp(absoluteTime, 0, 100)).ToString());
    }


    private void HandleCoinImageAnimation()
    {
        _currentCoinSize = Mathf.Lerp(_currentCoinSize, _coinSize, 4 * Time.deltaTime);
        _coinImage.sizeDelta = new Vector2(_currentCoinSize, _currentCoinSize);
    }

    public void DisableLinesEffect()
    {
        StartCoroutine(DisableLinesEffectRoutine());
    }

    public void DisableRedLinesEffect()
    {
        StartCoroutine(DisableRedLinesEffectRoutine());
    }

    private IEnumerator DisableLinesEffectRoutine()
    {
        yield return new WaitForSeconds(.5f);
        _linesEffect.SetActive(false);
    }

    private IEnumerator DisableRedLinesEffectRoutine()
    {
        yield return new WaitForSeconds(.5f);
        _linesRed.SetActive(false);
    }
}
