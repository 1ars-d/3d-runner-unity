using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenCratePieceController : MonoBehaviour
{
    private float _impulseStrength = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DisableCollider(0.001f));
        Destroy(gameObject, 0.4f);
    }

    private IEnumerator DisableCollider(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<MeshCollider>().enabled = false;
        GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-_impulseStrength, _impulseStrength), Random.Range(-_impulseStrength, _impulseStrength), Random.Range(_impulseStrength, 2*_impulseStrength)), ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
