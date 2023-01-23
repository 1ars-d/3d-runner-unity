using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{

    [SerializeField] private Material _playerMaterial;

    private void Start()
    {
        _playerMaterial.SetFloat("_FresnelPower", 20f);
    }

    public void PlayerGlowUp(Color color, float glodownDelay = 0)
    {
        StartCoroutine(PlayerGlowUpRoutine(color, glodownDelay));
    }

    private IEnumerator PlayerGlowUpRoutine(Color color, float glodownDelay)
    {
        _playerMaterial.SetColor("_FresnelColor", color);
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
        yield return new WaitForSeconds(glodownDelay);
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
