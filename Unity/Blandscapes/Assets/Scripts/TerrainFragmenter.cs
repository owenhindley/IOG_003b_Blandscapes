using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFragmenter : MonoBehaviour
{
    public GameObject sourceObject;
    public GameObject thufaPrefab;
    public Mesh sourceMesh; 

    public Material triangleMaterial;

    public List<GameObject> extractedTriangles;
    public int numTrianglesToExtract = 64;

    public float normalThreshold = 0.8f;
    public float separationDistance = 0.01f;
    public float capHeight = 0.2f;

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

            int index = Random.Range(0, numTriangles-1);
            Debug.Log("index : " + index);
            var vert0 = sourceMesh.triangles[index * 3];
            var vert1 = sourceMesh.triangles[(index*3)+1];
            var vert2 = sourceMesh.triangles[(index*3)+2];
            
            var pos0 = sourceMesh.vertices[vert0];
            var pos1 = sourceMesh.vertices[vert1];
            var pos2 = sourceMesh.vertices[vert2];

            var uv0_0 = sourceMesh.uv[vert0];
            var uv0_1 = sourceMesh.uv[vert1];
            var uv0_2 = sourceMesh.uv[vert2];

            var triNormal = sourceMesh.normals[vert0];

            pos0 += triNormal * separationDistance;
            pos1 += triNormal * separationDistance;
            pos2 += triNormal * separationDistance;

            if (Vector3.Dot(triNormal, Vector3.up) >= normalThreshold){

                var mid01 = Vector3.Lerp(pos0, pos1, 0.5f);
                var mid12 = Vector3.Lerp(pos1, pos2, 0.5f);
                var mid21 = Vector3.Lerp(pos2, pos1, 0.5f);

                var norm0 = sourceMesh.normals[vert0];
                var norm1 = sourceMesh.normals[vert0];
                var norm2 = sourceMesh.normals[vert0];


                var pivotPoint = mid01;
                // if (mid12.x < pivotPoint.x) pivotPoint = mid12;
                // if (mid21.x < pivotPoint.x) pivotPoint = mid21;

                pos0 -= pivotPoint;
                pos1 -= pivotPoint;
                pos2 -= pivotPoint;

                var rotNormalise = Quaternion.FromToRotation(pos1 - pos0, Vector3.left);
                pos0 = rotNormalise * pos0;
                pos1 = rotNormalise * pos1;
                pos2 = rotNormalise * pos2;

                
            
                
                var newMesh = new Mesh();
                newMesh.vertices = new Vector3[] { pos0, pos1, pos2};
                newMesh.uv = new Vector2[] { uv0_0, uv0_1, uv0_2};
                newMesh.normals = new Vector3[] { rotNormalise * norm0, rotNormalise * norm1, rotNormalise * norm2 };
                newMesh.triangles = new int[] { 0, 1, 2, 0, 2, 1 };


                var go = GameObject.Instantiate(thufaPrefab, Vector3.zero, Quaternion.identity);
                var mf = go.AddComponent<MeshFilter>();
                mf.mesh = newMesh;
                var newMr = go.AddComponent<MeshRenderer>();
                newMr.material = triangleMaterial;
                
                go.transform.parent = transform;
                go.transform.localPosition = pivotPoint;
                go.transform.localRotation = Quaternion.Inverse(rotNormalise);
                go.transform.localScale = Vector3.one;
                go.name = "TrianglePiece" + extractedTriangles.Count;

                extractedTriangles.Add(go);

                var cap = new GameObject();
                cap.name = "Cap"+extractedTriangles.Count;

                cap.transform.parent = go.transform;
                var capMf = cap.AddComponent<MeshFilter>();
                var capMr = cap.AddComponent<MeshRenderer>();

                capMf.mesh = CreateCapMesh(newMesh.vertices, newMesh.normals, newMesh.uv);
                capMr.material = triangleMaterial;

                cap.transform.localPosition = Vector3.zero;
                cap.transform.localRotation = Quaternion.identity;
                cap.transform.localScale = Vector3.one;

                var tt = go.GetComponent<ThufaTriangle>();
                tt.SetMidpoint(Vector3.Lerp(Vector3.Lerp(newMesh.vertices[0], newMesh.vertices[1], 0.5f), newMesh.vertices[2], 0.5f));

            }

            

        }

    }

    public Mesh CreateCapMesh(Vector3[] points, Vector3[] normals, Vector2[] uvs){
        var capMesh = new Mesh();

        var midVert = Vector3.Lerp(Vector3.Lerp(points[0], points[1], 0.5f), points[2], 0.5f);
        var midUV = Vector2.Lerp(Vector2.Lerp(uvs[0], uvs[1], 0.5f), uvs[2], 0.5f);
        var midNormal = Vector3.Lerp(Vector3.Lerp(normals[0], normals[1], 0.5f), normals[2], 0.5f);

        midVert += normals[0] * capHeight;

        capMesh.vertices = new Vector3[] { points[0], points[1], points[2], midVert };
        capMesh.uv = new Vector2[] { uvs[0], uvs[1], uvs[2], midUV };
        capMesh.normals = new Vector3[] { normals[0], normals[1], normals[2], midNormal};
        capMesh.triangles = new int[] { 0,1,2, 0, 2, 1, 0,3,2, 2, 3, 1, 0, 1, 3};


        return capMesh;
    }

    public void DestroyTriangles(){
        while(extractedTriangles.Count > 0){
            DestroyImmediate(extractedTriangles[0]);
            extractedTriangles.RemoveAt(0);
        }
    }
}
