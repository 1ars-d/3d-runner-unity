using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject _startMenu;
    [SerializeField] private GameObject _deathMenu;
    [SerializeField] private GameObject _playMenu;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private RectTransform _coinImage;
    [SerializeField] private Image _deathTimer;
    [SerializeField] private TextMeshProUGUI _deathTimerText;
    [SerializeField] private Image _energyBar;
    [SerializeField] private Image _energySymbol;

    [SerializeField] private Volume _PpVolume;
    private Vignette _vignette;


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
        _currentCoinSize = _coinSize;
        _energySymbolAnimator = _energySymbol.GetComponent<Animator>();
        _energySymbolAnimator.enabled = false;
        _gameManager = GetComponent<GameManager>();
        _PpVolume.profile.TryGet(out _vignette);
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

    // Update is called once per frame
    void Update()
    {
        HandleCoinImageAnimation();
        HandleEnergySymbol();
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
        _energyBar.fillAmount = value;
        _energyBar.color = new Color(1, value*1.3f, 0);
        _energySymbol.color = new Color(1, value*1.3f, 0);
    }

    public void SetMagnetBarFill(float value)
    {
        _magnetBar.fillAmount = value;
    }

    public void SetLinesEffectState(bool state)
    {
        _linesEffect.SetActive(state);
    }

    public void SetMagnetTimeLineState(bool state)
    {
        _magnetTimeline.SetActive(state);
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

    private IEnumerator SetDeathMenuRoutine(bool state)
    {
        yield return new WaitForSeconds(.15f);
        _deathMenu.SetActive(state);
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
        _deathTimerText.SetText(Mathf.CeilToInt(absoluteTime).ToString());
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

    private IEnumerator DisableLinesEffectRoutine()
    {
        yield return new WaitForSeconds(.5f);
        _linesEffect.SetActive(false);
    }
}
