using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;

public class GameOverGameManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _scoreValue;
    [SerializeField] private TextMeshProUGUI _coinValue;

    private float _currentDisplayScore;
    private float _currentDisplayCoins;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CountUpScoreToTarget(SceneVariables.LastGameScore, 1f));
        StartCoroutine(CountUpCoinsToTarget(SceneVariables.LastCollectedCoins, 1f));
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
            _coinValue.text = Mathf.RoundToInt(_currentDisplayCoins) + "";
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _coinValue.text = targetScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
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
