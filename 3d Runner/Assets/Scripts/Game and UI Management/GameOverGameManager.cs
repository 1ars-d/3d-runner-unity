using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;

public class GameOverGameManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _scoreValue;
    [SerializeField] private TextMeshProUGUI _gameCoinValue;
    [SerializeField] private TextMeshProUGUI _highScoreValue;
    [SerializeField] private GameObject _newHighScoreText;

    [SerializeField] private TextMeshProUGUI _totalCoins;
    [SerializeField] private TextMeshProUGUI _totalDiamonds;
    [SerializeField] private List<GameObject> _skins = new();


    [Header("Sound Effects")]
    [SerializeField] private AudioClip _buttonSound;
    [SerializeField] private AudioClip _scoreCountup;

    private float _currentDisplayScore;
    private float _currentDisplayCoins;

    // Start is called before the first frame update
    void Start()
    {
        _skins.ForEach(skin => skin.SetActive(false));
        _skins[PlayerPrefs.GetInt("_selectedSkin")].SetActive(true);
        StartCoroutine(CountUpScoreToTarget(SceneVariables.LastGameScore, 2.48f));
        StartCoroutine(CountUpCoinsToTarget(SceneVariables.LastCollectedCoins, 2.48f));
        _totalCoins.SetText(PlayerPrefs.GetInt("_playerCoins").ToString());
        _totalDiamonds.SetText(PlayerPrefs.GetInt("_playerDiamonds").ToString());
        SoundManager.Instance.PlaySound(_scoreCountup);
        HandleHighScore();
        _highScoreValue.SetText(PlayerPrefs.GetInt("_playerHighscore").ToString());
    }

    private void HandleHighScore()
    {
        if (SceneVariables.LastGameScore > PlayerPrefs.GetInt("_playerHighscore"))
        {
            PlayerPrefs.SetInt("_playerHighscore", SceneVariables.LastGameScore);
            _newHighScoreText.SetActive(true);
        }
    }

    IEnumerator CountUpScoreToTarget(int targetScore, float duration)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            _currentDisplayScore = Mathf.Lerp(0, targetScore, t); // or whatever to get the speed you like
            _scoreValue.text = Mathf.RoundToInt(_currentDisplayScore) + "";
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _scoreValue.text = targetScore.ToString();
    }

    IEnumerator CountUpCoinsToTarget(int targetScore, float duration)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            _currentDisplayCoins = Mathf.Lerp(0, targetScore, t); // or whatever to get the speed you like
            _gameCoinValue.text = Mathf.RoundToInt(_currentDisplayCoins) + "";
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _gameCoinValue.text = targetScore.ToString();
    }


    public void PlayButtonSound()
    {
        SoundManager.Instance.PlaySound(_buttonSound);
    }

    public void Play()
    {
        SceneVariables.StartGameOnMainLoad = true;
        SceneManager.LoadScene(0);
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
    }
}
