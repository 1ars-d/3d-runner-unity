using UnityEngine.UI;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [Header("Powerups")]
    [SerializeField] private float _magnetDuration;
    [SerializeField] private GameObject _rightArmMagnet;

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

    public void ActivateMagnet()
    {
        MagnetIsActive = true;
        _magnetTimer = _magnetDuration;
        _rightArmMagnet.SetActive(true);
        _UIController.SetMagnetTimeLineState(true);
        ItemCollectEffects();
    }

    public void ItemCollectEffects()
    {
        _playerEffects.PlayerGlowUp();
        _UIController.SetLinesEffectState(true);
        _UIController.DisableLinesEffect();
    }

    private void HandlePowerUps()
    {
        // Magnet
        if (_magnetTimer > 0)
        {
            _magnetTimer -= Time.deltaTime;
            _UIController.SetMagnetBarFill(_magnetTimer / _magnetDuration);
        }
        if (MagnetIsActive && _magnetTimer <= 0)
        {
            MagnetIsActive = false;
            _UIController.SetMagnetTimeLineState(false);
            _rightArmMagnet.SetActive(false);
        }
    }
}
