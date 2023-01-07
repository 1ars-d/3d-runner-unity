using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckMaterialController : MonoBehaviour
{

    [SerializeField] private List<Material> materials = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material mat = materials[Random.Range(0, materials.Count)];
        List<Material> matList = new List<Material>();
        matList.Add(mat);
        matList.Add(meshRenderer.materials[1]);
        matList.Add(meshRenderer.materials[2]);
        meshRenderer.materials = matList.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
