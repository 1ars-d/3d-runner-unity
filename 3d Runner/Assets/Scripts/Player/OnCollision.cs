using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollision : MonoBehaviour
{
    [Header("Collision Detection")]
    [SerializeField] private PlayerController m_char;
    [SerializeField] private UIController _UIController;
    private bool _isColliding;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("WoodenCrate") && m_char._inKick)
        {
            other.gameObject.GetComponent<WoodenCrateController>().DestroyCrate();
            StartCoroutine(_UIController.ChromaticAberation(1, 1f));
        } else if (other.gameObject.CompareTag("WoodenCrate"))
        {
            m_char.OnCharacterCollideHit(other);
            StartCoroutine(ResetColiding());
            return;
        }
        if (other.transform.tag == "InstantDeath")
        {
            m_char.OnInstantDeatHit();
            StartCoroutine(ResetColiding());
            return;
        }
        else if (other.transform.tag == "InstantWaterDeath")
        {
            m_char.OnInstantWaterDeatHit();
            StartCoroutine(ResetColiding());
            return;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isColliding || collision.transform.tag == "Player" || collision.transform.tag == "GroundTile" || collision.gameObject.CompareTag("CrateTile"))
        {
            return;
        }
        _isColliding = true;
        if (collision.transform.tag == "MoveableObstacle")
        {
            m_char.OnMoveableObstacleHit();
            StartCoroutine(ResetColiding());
            return;
        }
        m_char.OnCharacterCollideHit(collision.collider);
        StartCoroutine(ResetColiding());
    }

    IEnumerator ResetColiding()
    {
        yield return new WaitForSeconds(.001f);
        _isColliding = false;
    }
}
