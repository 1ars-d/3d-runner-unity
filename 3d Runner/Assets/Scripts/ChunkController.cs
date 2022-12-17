using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkController : MonoBehaviour
{
    void Update()
    {
        if (transform.position.z < -80f)
        {
            gameObject.GetComponentInParent<TerrainController>().ActiveChunks.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
