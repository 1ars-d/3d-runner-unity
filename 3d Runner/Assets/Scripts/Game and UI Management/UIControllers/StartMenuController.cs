using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _mainMenuCoins;
    [SerializeField] private TextMeshProUGUI _mainMenuDiamonds;
    [SerializeField] private TextMeshProUGUI _startMenuMultitplier;

    // Start is called before the first frame update
    void Start()
    {
        _mainMenuCoins.SetText(PlayerPrefs.GetInt("_playerCoins").ToString());
        _mainMenuDiamonds.SetText(PlayerPrefs.GetInt("_playerDiamonds").ToString());
        _startMenuMultitplier.SetText(PlayerPrefs.GetInt("_playerMultiplier").ToString());
    }

    private void OnEnable()
    {
        _mainMenuCoins.SetText(PlayerPrefs.GetInt("_playerCoins").ToString());
        _mainMenuDiamonds.SetText(PlayerPrefs.GetInt("_playerDiamonds").ToString());
        _startMenuMultitplier.SetText(PlayerPrefs.GetInt("_playerMultiplier").ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
