using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[DefaultExecutionOrder(10)]
public class CameraController : MonoBehaviour
{
    [Header("Camera Positions")]
    [SerializeField] private Vector3 _startCameraOffset;
    [SerializeField] private Vector3 _startCameraRotation;
    private Vector3 _leanCameraOffset;
    private Vector3 _leanCameraRotation;

    [Header("Camera Speed")]
    [SerializeField] private float _panDuration = 1f;
    [SerializeField] private float _panDelay = 0.6f;

    Animator _camAnimator;
    CinemachineTransposer transposer;

    // Start is called before the first frame update
    void Start()
    {
        _camAnimator = GetComponent<Animator>();
        transposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();
    }

    public void SetCameraPos()
    {
        _camAnimator.enabled = false;
        _leanCameraOffset = transposer.m_FollowOffset;
        SetLeanRotation();
        Debug.Log(_leanCameraOffset);
        Debug.Log(_leanCameraRotation);
        StartCoroutine(SetStartPosition(_panDuration));
        StartCoroutine(SetStartRotation(_panDuration));
    }

    private void SetLeanRotation()
    {
        Vector3 rotation = new Vector3();
        if (transform.eulerAngles.x <= 180f)
        {
            rotation.x = transform.eulerAngles.x;
        }
        else
        {
            rotation.x = transform.eulerAngles.x - 360f;
        }
        if (transform.eulerAngles.y <= 180f)
        {
            rotation.y = transform.eulerAngles.y;
        }
        else
        {
            rotation.y = transform.eulerAngles.y - 360f;
        }
        if (transform.eulerAngles.z <= 180f)
        {
            rotation.z = transform.eulerAngles.z;
        }
        else
        {
            rotation.z = transform.eulerAngles.z - 360f;
        }
        _leanCameraRotation = rotation;
    }

    private IEnumerator SetStartPosition(float duration)
    {
        yield return new WaitForSeconds(_panDelay);
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            transposer.m_FollowOffset = Vector3.Lerp(_leanCameraOffset, _startCameraOffset, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator SetStartRotation(float duration)
    {
        yield return new WaitForSeconds(_panDelay);
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            transform.rotation = Quaternion.Euler(Vector3.Lerp(_leanCameraRotation, _startCameraRotation, t));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalOffset = transposer.m_FollowOffset;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transposer.m_FollowOffset = new Vector3(originalOffset.x + x, originalOffset.y + y, originalOffset.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transposer.m_FollowOffset = originalOffset;
    }
}
