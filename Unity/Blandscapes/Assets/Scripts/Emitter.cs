using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter : MonoBehaviour
{
    public GameObject thufaPrefab;

    public float EmitSize = 20.0f;

    public int thufaCount = 256;
    public float thufaScale = 2.0f;

    public AnimationCurve scaleCurve;

    public List<GameObject> thufaList;

    public int numPhysicsSteps = 20;

    [InspectorButton("CreateThufa")] public bool doCreateThufa;
    [InspectorButton("DeleteAllThufa")] public bool doDeleteAllThufa;
    [InspectorButton("RunPhysics")] public bool doRunPhysics;

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

    [ContextMenu("CreateThufa")]
    public void CreateThufa(){
        for (int i=0; i < thufaCount; i++){
            var pos = new Vector3(Random.Range(-EmitSize, EmitSize)/2.0f, 0.0f, Random.Range(-EmitSize, EmitSize)/2.0f);
            var th = GameObject.Instantiate(thufaPrefab, Vector3.zero, Quaternion.identity, transform);
            th.transform.localPosition = pos;
            th.transform.Rotate(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
            th.transform.localScale = Vector3.one * scaleCurve.Evaluate(Random.value) * thufaScale;
            thufaList.Add(th);
        }

    }

    [ContextMenu("DeleteAll")]
    public void DeleteAllThufa(){
        while (thufaList.Count > 0){
            DestroyImmediate(thufaList[0]);
            thufaList.RemoveAt(0);
        }
    }

    [ContextMenu("RunPhysics")]
    public void RunPhysics(){

        for (int i=0; i<thufaList.Count; i++){
            var rb = thufaList[i].GetComponent<Rigidbody>().isKinematic = false;
        }
        Physics.autoSimulation = false;
        for (int i=0; i < numPhysicsSteps; i++){
            Physics.Simulate(0.01f);
        }

        Physics.autoSimulation = true;

        for (int i=0; i<thufaList.Count; i++){
            var rb = thufaList[i].GetComponent<Rigidbody>().isKinematic = true;
        }

    }
}
