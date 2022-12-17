using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopController : MonoBehaviour
{
    [SerializeField] private Vector3 _endPos;
    [SerializeField] private Vector3 _endRotation;
    [SerializeField] private float _moveTime;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StartMoving()
    {
        animator.Play("StartRunning");
        StartCoroutine(MoveTowardsPlayer(_moveTime));
    }

    IEnumerator MoveTowardsPlayer(float duration)
    {
        float timeElapsed = 0;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            transform.position = Vector3.Lerp(startPos, _endPos, t);
            transform.rotation = Quaternion.Euler(Vector3.Lerp(new Vector3(startRot.x, startRot.y, startRot.z), _endRotation, t));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(BackUp(1f));
    }

    IEnumerator BackUp(float duration)
    {
        float timeElapsed = 0;
        Vector3 startPos = transform.position;
        Vector3 backUpPos = new Vector3(startPos.x, startPos.y, startPos.z - 2f);
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            transform.position = Vector3.Lerp(startPos, backUpPos, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
