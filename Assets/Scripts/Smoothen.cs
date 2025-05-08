using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class MeshSmoother : MonoBehaviour
{
    public Vector3 worldCenter;
    public float radius = 1f;
    public int smoothingIterations = 3;

    private Mesh mesh;
    private Vector3[] verts;
    private List<int>[] neighbors;

    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (!mf) { Debug.LogError("Missing MeshFilter!"); return; }

        // Instantiate a unique copy so we can edit it
        Mesh runtimeMesh = Instantiate(mf.sharedMesh);
        runtimeMesh.name = mf.sharedMesh.name + "_RuntimeCopy";
        mf.mesh = runtimeMesh;

        mesh = runtimeMesh;
        verts = mesh.vertices;
        BuildNeighborLookup();

        SmoothArea();

        mesh.vertices = verts;
        mesh.RecalculateNormals();
    }

    // ... rest of BuildNeighborLookup, AddNeighbor, SmoothArea as before ...

    void BuildNeighborLookup()
    {
        // Build adjacency list: for each vertex, which other verts share an edge?
        neighbors = new List<int>[verts.Length];
        for (int i = 0; i < verts.Length; i++)
            neighbors[i] = new List<int>();

        int[] tris = mesh.triangles;
        for (int t = 0; t < tris.Length; t += 3)
        {
            int a = tris[t], b = tris[t+1], c = tris[t+2];
            // add each edge both ways
            AddNeighbor(a,b); AddNeighbor(b,a);
            AddNeighbor(a,c); AddNeighbor(c,a);
            AddNeighbor(b,c); AddNeighbor(c,b);
        }
    }

    void AddNeighbor(int from, int to)
    {
        if (!neighbors[from].Contains(to))
            neighbors[from].Add(to);
    }

    void SmoothArea()
    {
        Vector3 localCenter = transform.InverseTransformPoint(worldCenter);

        for (int iter = 0; iter < smoothingIterations; iter++)
        {
            Vector3[] newVerts = (Vector3[])verts.Clone();

            for (int i = 0; i < verts.Length; i++)
            {
                // only smooth verts within radius of the center
                if ((verts[i] - localCenter).magnitude > radius) continue;

                // average the positions of neighbors
                Vector3 sum = Vector3.zero;
                foreach (int n in neighbors[i])
                    sum += verts[n];
                sum /= neighbors[i].Count;

                // lerp towards that average
                newVerts[i] = Vector3.Lerp(verts[i], sum, 0.5f);
            }

            verts = newVerts;
        }
    }
}
