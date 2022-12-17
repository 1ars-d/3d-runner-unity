using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollision : MonoBehaviour
{
    [Header("Collision Detection")]
    [SerializeField] private PlayerController m_char;
    private bool _isColliding;

    private void OnCollisionEnter(Collision collision)
    {
        if (_isColliding || collision.transform.tag == "Player" || collision.transform.tag == "GroundTile")
        {
            return;
        }
        _isColliding = true;
        if (collision.transform.tag == "MoveableObstacle")
        {
            m_char.OnMoveableObstacleHit();
            return;
        }
        m_char.OnCharacterCollideHit(collision.collider);
        StartCoroutine(ResetColiding());
    }

    IEnumerator ResetColiding()
    {
        yield return new WaitForSeconds(.01f);
        _isColliding = false;
    }
}
