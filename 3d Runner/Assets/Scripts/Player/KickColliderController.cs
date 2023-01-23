using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickColliderController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.CompareTag("WoodenCrate"))
        {
            collision.gameObject.GetComponent<WoodenCrateController>().DestroyCrate();
        } else if (!collision.gameObject.CompareTag("Player"))
        {
            gameObject.GetComponentInParent<PlayerController>().OnCharacterCollideHit(collision.collider);
        }
    }
}
