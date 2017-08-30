using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class XRSurfaceController : MonoBehaviour {

  // If true, XRSurfaceController will update the rendered mesh and the collider mesh of the surface
  // so that it matches the detected surface. This allows for interactions like shadows that clip
  // to surface boundaries, and objects that can fall off surfaces.
  public bool deformToSurface = false;

  private XRController xr;

  private MeshFilter meshFilter = null;
  private MeshCollider meshCollider = null;

  private long surfaceId = Int64.MinValue;

  void Start() {
    xr = GameObject.FindWithTag("XRController").GetComponent<XRController>();
    // Add MeshFilter and MeshCollider if not already added.
    meshFilter = gameObject.GetComponent<MeshFilter>();
    if (deformToSurface && meshFilter == null) {
      meshFilter = gameObject.AddComponent<MeshFilter>();
    }

    meshCollider = gameObject.GetComponent<MeshCollider>();
    if (deformToSurface && meshCollider == null) {
      meshCollider = gameObject.AddComponent<MeshCollider>();
      meshCollider.sharedMesh = meshFilter.mesh;
    }

    // Start the surface very far in the distance until there is a detected surface.
    transform.position = new Vector3(10000, 0, 10000);
  }

  private void UpdateMesh(long id, Mesh mesh) {
    // If we are switching planes, set the transform to be the vertex center of the new plane.
    if (id != surfaceId) {
      double x = 0.0;
      double y = 0.0;
      double z = 0.0;
      foreach (Vector3 vertex in mesh.vertices) {
        x += vertex.x;
        y += vertex.y;
        z += vertex.z;
      }
      double il = 1.0 / mesh.vertices.Length;
      Vector3 vertexCenter = new Vector3((float)(x * il), (float)(y * il), (float)(z * il));
      transform.position = vertexCenter;
    }

    surfaceId = id;
    if (!deformToSurface) {
      return;
    }

    // Set the mesh vertices relative to the transform center.
    Mesh relMesh = new Mesh();
    Vector3[] relVertices = new Vector3[mesh.vertices.Length];
    Vector3 anchor = transform.position;
    for (int i = 0; i < mesh.vertices.Length; ++i) {
      relVertices[i] = new Vector3(
        mesh.vertices[i].x - anchor.x,
        0.0f,
        mesh.vertices[i].z - anchor.z);
    }
    relMesh.vertices = relVertices;
    relMesh.normals = mesh.normals;
    relMesh.uv = mesh.uv;
    relMesh.triangles = mesh.triangles;

    meshFilter.mesh = relMesh;
    meshCollider.sharedMesh = relMesh;
  }

  void Update() {
    // If there are no meshes, reset the id to default and don't change
    // anything.
    Mesh mesh = xr.GetActiveSurfaceMesh();
    if (mesh == null) {
      surfaceId = Int64.MinValue;
      return;
    }

    UpdateMesh(xr.GetActiveSurfaceId(), mesh);
  }
}
