using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;


public class SkinChangeController : MonoBehaviour
{

    [System.Serializable]
    public class Price
    {
        public string currency;
        public int price;
    }

    [System.Serializable]
    public class Prices
    {
        public Price[] prices;
    }

    public Prices _skinPrices = new();

    public List<GameObject> _skins = new();
    public TextAsset pricesJsonFile;
    [SerializeField] private TextMeshProUGUI _totalCoins;
    [SerializeField] private TextMeshProUGUI _totalDiamonds;
    [SerializeField] private Button _selectSkinButton;
    [SerializeField] private Button _selectedSkinButton;
    [SerializeField] private Button _buySkinButton;
    [SerializeField] private TextMeshProUGUI _buySkinButtonText;
    [SerializeField] private GameObject _buyButtonDiamondIcon;
    [SerializeField] private GameObject _buyButtonCoinIcon;
    [SerializeField] private GameObject _lockBadge;
    [SerializeField] private UIController _UIController;

    private int _currentSkinIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        _totalCoins.SetText(PlayerPrefs.GetInt("_playerCoins").ToString());
        _totalDiamonds.SetText(PlayerPrefs.GetInt("_playerDiamonds").ToString());
        ReadPrices();
        _currentSkinIndex = PlayerPrefs.GetInt("_selectedSkin");
        if (_currentSkinIndex == PlayerPrefs.GetInt("_selectedSkin"))
        {
            _selectedSkinButton.gameObject.SetActive(true);
            _selectedSkinButton.gameObject.SetActive(true);
        }
        else
        {
            _selectedSkinButton.gameObject.SetActive(true);
            _selectSkinButton.gameObject.SetActive(true);
        }
        _skins[_currentSkinIndex].SetActive(true);
    }

    private void ReadPrices()
    {
        _skinPrices = JsonUtility.FromJson<Prices>(pricesJsonFile.text);
    }


    public void ActivateSelectedSkin()
    {
        _skins.ForEach(skin => skin.SetActive(false));
        _skins[PlayerPrefs.GetInt("_selectedSkin")].SetActive(true);
    }

    public void ChangeSkinLeft()
    {
        _skins[_currentSkinIndex].SetActive(false);
        if (_currentSkinIndex > 0)
        {
            _currentSkinIndex--;
        } else
        {
            _currentSkinIndex = _skins.Count - 1;
        }
        _skins.ForEach(x => x.SetActive(false));
        _skins[_currentSkinIndex].SetActive(true);
        SetMainButtonState();
    }

    public void ChangeSkinRight()
    {
        _skins[_currentSkinIndex].SetActive(false);
        if (_currentSkinIndex < _skins.Count - 1)
        {
            _currentSkinIndex++;
        }
        else
        {
            _currentSkinIndex = 0;
        }
        _skins.ForEach(x => x.SetActive(false));
        _skins[_currentSkinIndex].SetActive(true);
        SetMainButtonState();
    }

    public void OnSelect()
    {
        PlayerPrefs.SetInt("_selectedSkin", _currentSkinIndex);
        _selectedSkinButton.gameObject.SetActive(true);
        _selectSkinButton.gameObject.SetActive(false);
    }

    public void OnBuy()
    {
        if (_skinPrices.prices[_currentSkinIndex].currency == "d")
        {
            int price = _skinPrices.prices[_currentSkinIndex].price;
            int _lastTotalDiamonds = PlayerPrefs.GetInt("_playerDiamonds");
            if (_lastTotalDiamonds < price)
            {
                _UIController.OpenModal("You don't have enough diamonds to buy this!", "Get More", () => _UIController.CloseModal());
                return;
            }
            PlayerPrefs.SetInt("_playerDiamonds", _lastTotalDiamonds - price);
            _totalDiamonds.SetText((_lastTotalDiamonds - price).ToString());
        } else
        {
            int price = _skinPrices.prices[_currentSkinIndex].price;
            int _lastTotalCoins = PlayerPrefs.GetInt("_playerCoins");
            if (_lastTotalCoins < price)
            {
                _UIController.OpenModal("You don't have enough coins to buy this!", "Get More", () => _UIController.CloseModal());
                return;
            }
            PlayerPrefs.SetInt("_playerCoins", _lastTotalCoins - price);
            _totalCoins.SetText((_lastTotalCoins - price).ToString());
        }
        PlayerPrefs.SetInt("_skin_" + _currentSkinIndex.ToString(), 1);
        SetMainButtonState();
    }

    private void SetMainButtonState()
    {
        if (_currentSkinIndex == PlayerPrefs.GetInt("_selectedSkin"))
        {
            _selectedSkinButton.gameObject.SetActive(true);
            _selectSkinButton.gameObject.SetActive(false);
            _buySkinButton.gameObject.SetActive(false);
            _lockBadge.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("_skin_" + _currentSkinIndex.ToString()) == 1)
        {
            _selectSkinButton.gameObject.SetActive(true);
            _selectedSkinButton.gameObject.SetActive(false);
            _buySkinButton.gameObject.SetActive(false);
            _lockBadge.SetActive(false);
        }
        else
        {
            _buySkinButton.gameObject.SetActive(true);
            _selectSkinButton.gameObject.SetActive(false);
            _selectedSkinButton.gameObject.SetActive(false);
            _lockBadge.SetActive(true);
            _buySkinButtonText.SetText(_skinPrices.prices[_currentSkinIndex].price.ToString());
            if (_skinPrices.prices[_currentSkinIndex].currency == "d")
            {
                _buyButtonDiamondIcon.SetActive(true);
                _buyButtonCoinIcon.SetActive(false);
            } else
            {
                _buyButtonCoinIcon.SetActive(true);
                _buyButtonDiamondIcon.SetActive(false);
            }
         }
    }
}
