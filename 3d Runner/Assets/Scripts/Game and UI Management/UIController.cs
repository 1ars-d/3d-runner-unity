using System.Collections;
using System.Collections.Generic;
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

    [Header("Powerup UI")]
    [SerializeField] private GameObject _magnetTimeline;
    [SerializeField] private Image _magnetBar;
    [SerializeField] private GameObject _linesEffect;

    [Header("Coin Image")]
    [SerializeField] private float _coinSize = 56.358f;
    [SerializeField] private float _largeCoinSize = 70f;

    private float _currentCoinSize;

    // Start is called before the first frame update
    void Start()
    {
        _currentCoinSize = _coinSize;
    }

    // Update is called once per frame
    void Update()
    {
        HandleCoinImageAnimation();
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

    public void SetDeathTimer(float value)
    {
        _deathTimer.fillAmount = value;
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
