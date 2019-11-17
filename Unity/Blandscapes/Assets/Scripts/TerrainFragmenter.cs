using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFragmenter : MonoBehaviour
{
    public GameObject sourceObject;
    public Mesh sourceMesh; 

    public Material triangleMaterial;

    public List<GameObject> extractedTriangles;
    public int numTrianglesToExtract = 64;

    public float normalThreshold = 0.8f;

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

        int numTriangles = sourceMesh.triangles.Length / 3;
        Debug.Log("numTriangles : " + numTriangles);

        while(extractedTriangles.Count < numTrianglesToExtract) {

            int index = Random.Range(0, numTriangles);
            Debug.Log("index : " + index);
            var vert0 = sourceMesh.triangles[index * 3];
            var vert1 = sourceMesh.triangles[(index*3)+1];
            var vert2 = sourceMesh.triangles[(index*3)+2];
            
            var pos0 = sourceMesh.vertices[vert0];
            var pos1 = sourceMesh.vertices[vert1];
            var pos2 = sourceMesh.vertices[vert2];

            var uv0_0 = sourceMesh.uv[index];
            var uv0_1 = sourceMesh.uv[index+2];
            var uv0_2 = sourceMesh.uv[index+3];

            var triNormal = sourceMesh.normals[index];

            if (Vector3.Dot(triNormal, Vector3.up) >= normalThreshold){

                var mid01 = Vector3.Lerp(pos0, pos1, 0.5f);
                var mid12 = Vector3.Lerp(pos1, pos2, 0.5f);
                var mid21 = Vector3.Lerp(pos2, pos1, 0.5f);


                var pivotPoint = mid01;
                if (mid12.x < pivotPoint.x) pivotPoint = mid12;
                if (mid21.x < pivotPoint.x) pivotPoint = mid21;

                pos0 -= pivotPoint;
                pos1 -= pivotPoint;
                pos2 -= pivotPoint;
                
                var newMesh = new Mesh();
                newMesh.vertices = new Vector3[] { pos0, pos1, pos2};
                newMesh.uv = new Vector2[] { uv0_0, uv0_1, uv0_2};
                newMesh.triangles = new int[] { 0, 1, 2, 0, 2, 1};
                

                var go = new GameObject();
                var mf = go.AddComponent<MeshFilter>();
                mf.mesh = newMesh;
                var newMr = go.AddComponent<MeshRenderer>();
                newMr.material = triangleMaterial;
                
                go.transform.parent = transform;
                go.transform.localPosition = pivotPoint;
                go.transform.localScale = Vector3.one;
                go.name = "TrianglePiece" + extractedTriangles.Count;

                extractedTriangles.Add(go);

            }

            

        }

    }

    public void DestroyTriangles(){
        while(extractedTriangles.Count > 0){
            DestroyImmediate(extractedTriangles[0]);
            extractedTriangles.RemoveAt(0);
        }
    }
}
