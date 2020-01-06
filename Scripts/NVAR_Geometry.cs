using System.Collections;
using System.Collections.Generic;
using NVIDIA.VRWorksAudio.Internal;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class NVAR_Geometry : MonoBehaviour {
    private NVAR_Listener listener;
    private NVAR.Mesh NVAR_mesh;
    private Mesh mesh;

    internal NVAR.Material material;
	// Use this for initialization
	void Start () {
        listener = Camera.main.gameObject.GetComponent<NVAR_Listener>();
        mesh = gameObject.GetComponent<MeshFilter>().mesh;
        NVAR.CreateMaterial(listener.context, out material);
        NVAR.CreateMesh(listener.context, out NVAR_mesh, gameObject.transform.localToWorldMatrix, mesh.vertices, mesh.triangles, mesh.triangles.Length / 3, material);
    }
}
