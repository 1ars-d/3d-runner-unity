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
    [SerializeField] private Vector3 _lookAtCopPosition;
    [SerializeField] private Vector3 _lookAtCopRotation;
    [SerializeField] private float _startFOV;
    [SerializeField] private float _endFOV;
    private Vector3 _leanCameraOffset;
    private Vector3 _leanCameraRotation;
    [SerializeField] private GameObject _copSkin;


    [Header("Camera Speed")]
    [SerializeField] private float _panDuration = 1f;
    [SerializeField] private float _lookAtCopDuration = 0.3f;
    [SerializeField] private float _panDelay = 0.6f;

    Animator _camAnimator;
    CinemachineVirtualCamera v_cam;
    CinemachineTransposer transposer;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GameObject _copCar1;
    [SerializeField] private GameObject _copCar2;

    // Start is called before the first frame update
    void Start()
    {
        _copCar1.SetActive(false);
        _copCar2.SetActive(false);
        _copSkin.SetActive(false);
        _camAnimator = GetComponent<Animator>();
        transposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();
        v_cam = GetComponent<CinemachineVirtualCamera>();
    }

    public void SetCameraPos()
    {
        _camAnimator.enabled = false;
        _leanCameraOffset = transposer.m_FollowOffset;
        Debug.Log(_leanCameraOffset);
        SetLeanRotation();
        StartCoroutine(LookAtCop(_lookAtCopDuration));
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

    private IEnumerator LookAtCop(float duration)
    {
        _copCar1.SetActive(true);
        _copCar2.SetActive(true);
        _playerController.StartLookAnimation();
        StartCoroutine(StartPlayer(duration * 0.7f));
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            v_cam.m_Lens.FieldOfView = Mathf.Lerp(_startFOV, _endFOV, t);
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(_leanCameraRotation), Quaternion.Euler(_lookAtCopRotation), t);
            transposer.m_FollowOffset = Vector3.Lerp(_leanCameraOffset, _lookAtCopPosition, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = Quaternion.Euler(_lookAtCopRotation);
        transposer.m_FollowOffset = _lookAtCopPosition;
        StartCoroutine(SetStartPosition(_panDuration));
        StartCoroutine(SetStartRotation(_panDuration));
    }

    private IEnumerator StartPlayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        _playerController.StartRunning();
    }

    public IEnumerator EaseDistanceOnDeath(float distance, float duration)
    {
        float timeElapsed = 0;
        float _prevZOffset = transposer.m_FollowOffset.z;
        float _newZOffset = transposer.m_FollowOffset.z + distance;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            transposer.m_FollowOffset = Vector3.Lerp(new Vector3(transposer.m_FollowOffset.x, transposer.m_FollowOffset.y, _prevZOffset), new Vector3(transposer.m_FollowOffset.x, transposer.m_FollowOffset.y, _newZOffset), t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator SetStartPosition(float duration)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            transposer.m_FollowOffset = Vector3.Lerp(_lookAtCopPosition, _startCameraOffset, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator SetStartRotation(float duration)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            transform.rotation = Quaternion.Euler(Vector3.Lerp(_lookAtCopRotation, _startCameraRotation, t));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _copSkin.SetActive(true);
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
