using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DG_InvertMesh : MonoBehaviour {

    private void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = mesh.triangles.Reverse().ToArray();
    }
}
