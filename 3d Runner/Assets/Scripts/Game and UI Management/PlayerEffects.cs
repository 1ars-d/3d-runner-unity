using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{

    [SerializeField] private Material _playerMaterial;

    public void PlayerGlowUp()
    {
        StartCoroutine(PlayerGlowUpRoutine());
    }

    private IEnumerator PlayerGlowUpRoutine()
    {
        _playerMaterial.SetColor("_FresnelColor", new Color(1, 0.7375139f, 0.25f, 1));
        float duration = 0.15f;
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            _playerMaterial.SetFloat("_FresnelPower", Mathf.Lerp(20, 1.5f, t));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(PlayerGlowDown());
    }

    private IEnumerator PlayerGlowDown()
    {
        float duration = 0.2f;
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            _playerMaterial.SetFloat("_FresnelPower", Mathf.Lerp(1.5f, 20, t));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _playerMaterial.SetColor("_FresnelColor", new Color(0, 0, 0, 0));
    }
}
