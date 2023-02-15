using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenCratePieceController : MonoBehaviour
{
    private float _impulseStrength = 4f;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-_impulseStrength, _impulseStrength), Random.Range(-_impulseStrength, _impulseStrength), Random.Range(_impulseStrength, 2 * _impulseStrength)), ForceMode.Impulse);
        Destroy(gameObject, 0.4f);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
