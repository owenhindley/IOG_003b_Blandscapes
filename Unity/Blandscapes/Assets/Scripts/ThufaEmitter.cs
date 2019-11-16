using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThufaEmitter : MonoBehaviour
{
    public GameObject thufaPrefab;

    public float EmitSize = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmosSelected(){

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(EmitSize, 0.1f, EmitSize));

    }
}
