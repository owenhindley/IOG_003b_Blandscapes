using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFragmenter : MonoBehaviour
{
    public GameObject sourceObject;
    public Mesh sourceMesh; 

    public List<GameObject> extractedTriangles;
    public int numTrianglesToExtract = 64;

    [InspectorButton("GrabTriangles")] public bool DoGrabTriangles;
    [InspectorButton("DestroyTriangles")] public bool DoDestroyTriangles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GrabTriangles(){

        for (int i=0; i < numTrianglesToExtract; i++){
            int index = Mathf.FloorToInt(Random.value * sourceMesh.triangles.Length / 3);
            var vert0 = sourceMesh.triangles[index];
            var vert1 = sourceMesh.triangles[index+1];
            var vert2 = sourceMesh.triangles[index+2];
            
            var pos0 = sourceMesh.vertices[vert0];
            var pos1 = sourceMesh.vertices[vert1];
            var pos2 = sourceMesh.vertices[vert2];

            var uv0_0 = sourceMesh.uv[index];
            var uv0_1 = sourceMesh.uv[index+2];
            var uv0_2 = sourceMesh.uv[index+3];

            var newMesh = new Mesh();
            newMesh.vertices = new Vector3[] { pos0, pos1, pos2};
            newMesh.uv = new Vector2[] { uv0_0, uv0_1, uv0_2};
            newMesh.triangles = new int[] { 0, 1, 2, 0, 2, 1};

            var go = new GameObject();
            var mf = go.AddComponent<MeshFilter>();
            mf.mesh = newMesh;
            var newMr = go.AddComponent<MeshRenderer>();
            newMr.material = sourceObject.GetComponent<MeshRenderer>().sharedMaterial;
            
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.name = "TrianglePiece" + i;

            extractedTriangles.Add(go);

        }

    }

    public void DestroyTriangles(){
        while(extractedTriangles.Count > 0){
            DestroyImmediate(extractedTriangles[0]);
            extractedTriangles.RemoveAt(0);
        }
    }
}
