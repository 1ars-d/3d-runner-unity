using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnerController : MonoBehaviour
{

    [SerializeField] private List<GameObject> spawns = new List<GameObject>();
    [SerializeField] private int _probabilty = 4;


    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        int random = Random.Range(0, spawns.Count * _probabilty);
        if (random < spawns.Count)
        {
            Instantiate(spawns[random], new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.Euler(new Vector3(0, -90, 0)), transform);
        }
    }
}
