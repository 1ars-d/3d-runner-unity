using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _totalCoins;
    [SerializeField] private TextMeshProUGUI _totalDiamonds;

    [Header("Magnet")]
    [SerializeField] private TextMeshProUGUI _magnetPriceText;
    [SerializeField] private Image _magnetBar;
    [SerializeField] private Button _magnetBuy;
    [SerializeField] private int _magnetPrice;

    [Header("Multiplier")]
    [SerializeField] private TextMeshProUGUI _multiplierPriceText;
    [SerializeField] private Image _multiplierBar;
    [SerializeField] private Button _multiplierBuy;
    [SerializeField] private int _multiplierPrice;


    // Start is called before the first frame update
    void Start()
    {
        _magnetPriceText.SetText(_multiplierPrice.ToString());
        _multiplierPriceText.SetText(_multiplierPrice.ToString());
        _totalCoins.SetText(PlayerPrefs.GetInt("_playerCoins").ToString());
        _totalDiamonds.SetText(PlayerPrefs.GetInt("_playerDiamonds").ToString());
        int _multiplierLevel = PlayerPrefs.GetInt("_multiplierDuration");
        int _magnetLevel = PlayerPrefs.GetInt("_magnetDuration");
        if (_multiplierLevel >= 24)
        {
            _multiplierBuy.interactable = false;
        }
        if (_magnetLevel >= 24)
        {
            _magnetBuy.interactable = false;
        }
        _magnetBar.fillAmount = (float)_magnetLevel / 24;
        _multiplierBar.fillAmount = (float)_multiplierLevel / 24;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MagnetBuy()
    {
        int _lastTotalCoins = PlayerPrefs.GetInt("_playerCoins");
        if (_lastTotalCoins < _magnetPrice) return;
        PlayerPrefs.SetInt("_playerCoins", _lastTotalCoins - _magnetPrice);
        _totalCoins.SetText((_lastTotalCoins - _magnetPrice).ToString());
        int _currentLevel = PlayerPrefs.GetInt("_magnetDuration");
        PlayerPrefs.SetInt("_magnetDuration", _currentLevel + 1);
        _magnetBar.fillAmount = (float)(_currentLevel + 1) / 24;
        if (_currentLevel + 1 >= 24)
        {
            _magnetBuy.interactable = false;
        }
    }

    public void MultiplierBuy()
    {
        int _lastTotalCoins = PlayerPrefs.GetInt("_playerCoins");
        if (_lastTotalCoins < _multiplierPrice) return;
        PlayerPrefs.SetInt("_playerCoins", _lastTotalCoins - _multiplierPrice);
        _totalCoins.SetText((_lastTotalCoins - _multiplierPrice).ToString());
        int _currentLevel = PlayerPrefs.GetInt("_multiplierDuration");
        PlayerPrefs.SetInt("_multiplierDuration", _currentLevel + 1);
        _multiplierBar.fillAmount = (float)(_currentLevel + 1) / 24;
        if (_currentLevel + 1 >= 24)
        {
            _multiplierBuy.interactable = false;
        }
    }
}
