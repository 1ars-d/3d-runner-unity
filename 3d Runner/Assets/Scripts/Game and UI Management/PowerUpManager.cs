using UnityEngine.UI;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [Header("Powerups")]
    [SerializeField] private float _magnetBaseDuration;
    [SerializeField] private float _magnetAddDuration;
    [SerializeField] private GameObject _rightArmMagnet;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip _powerupRunOutEffect;

    [HideInInspector] public bool MagnetIsActive;
    private float _magnetTimer;

    private UIController _UIController;
    private PlayerEffects _playerEffects;

    // Start is called before the first frame update
    void Start()
    {
        _UIController = GetComponent<UIController>();
        _playerEffects = GetComponent<PlayerEffects>();
    }

    // Update is called once per frame
    void Update()
    {
        HandlePowerUps();
    }

    public void DeactivateMagnet()
    {
        MagnetIsActive = false;
        _UIController.SetMagnetTimeLineState(false);
        _rightArmMagnet.SetActive(false);
    }

    public void ActivateMagnet()
    {
        MagnetIsActive = true;
        _magnetTimer = _magnetBaseDuration + PlayerPrefs.GetInt("_magnetDuration") * _magnetAddDuration;
        _rightArmMagnet.SetActive(true);
        _UIController.SetMagnetTimeLineState(true);
        ItemCollectEffects();
    }

    public void ItemCollectEffects()
    {
        _playerEffects.PlayerGlowUp(new Color(1, 0.7375139f, 0.25f, 1));
        _UIController.SetLinesEffectState(true);
        _UIController.DisableLinesEffect();
    }

    public void OnDiamondCollectEffect()
    {
        _playerEffects.PlayerGlowUp(new Color(1, 1, 1, 0.8f));
        _UIController.SetRedLinesEffectState(true);
        _UIController.DisableRedLinesEffect();
    }

    private void HandlePowerUps()
    {
        // Magnet
        if (_magnetTimer > 0)
        {
            _magnetTimer -= Time.deltaTime;
            _UIController.SetMagnetBarFill(_magnetTimer / (_magnetBaseDuration + PlayerPrefs.GetInt("_magnetDuration") * _magnetAddDuration));
        }
        if (MagnetIsActive && _magnetTimer <= 0)
        {
            SoundManager.Instance.PlaySound(_powerupRunOutEffect);
            MagnetIsActive = false;
            _UIController.SetMagnetTimeLineState(false);
            _rightArmMagnet.SetActive(false);
        }
    }
}
