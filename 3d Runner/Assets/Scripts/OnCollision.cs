using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollision : MonoBehaviour
{
    [Header("Collision Detection")]
    [SerializeField] private PlayerController m_char;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            return;
        }
        if (collision.transform.tag == "MoveableObstacle")
        {
            m_char.OnMoveableObstacleHit();
            return;
        }
        m_char.OnCharacterCollideHit(collision.collider);
    }
}
