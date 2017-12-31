using UnityEngine;
using System;
using System.Collections.Generic;

public class YMeshUtilities
{
    public static void GetAndApplyMeshFromSprite(Sprite s, float depth, MeshRenderer mr, MeshFilter mf)
    {
        Mesh new_mesh = GetMeshFromSprite(s, depth);
        Material new_mat = new Material(Shader.Find("Unlit/Texture"));
        mr.material = new_mat;
        mr.material.mainTexture = s.texture;
        mf.mesh = new_mesh;
    }

    public static void GetAndApplyMeshFromTexture(Texture t, float depth, MeshRenderer mr, MeshFilter mf, float pixels_per_unit=100)
    {
        Mesh new_mesh = GetMeshFromTexture(t, depth, pixels_per_unit);
        Material new_mat = new Material(Shader.Find("Unlit/Texture"));
        mr.material = new_mat;
        mr.material.mainTexture = t;
        mf.mesh = new_mesh;
    }

    public static Mesh GetMeshFromTexture(Texture t, float depth, float pixels_per_unit=100)
    {
        Mesh mesh = new Mesh();

        Sprite s = Sprite.Create(
            (Texture2D)t,
            new Rect(0, 0, t.width, t.height),
            new Vector2(0.5f, 0.5f),
            pixels_per_unit,
            0,
            SpriteMeshType.Tight
        );
        
        mesh.vertices = GetVerticesFromSprite2(s, depth).ToArray();
        mesh.uv = GetUVsFromSprite2(s).ToArray();
        mesh.triangles = GetTrisFromSprite2(s).ToArray();

        /*Debug.Log("Vertices: " + mesh.vertices.Length.ToString());
        for (int i = 0; i < mesh.vertices.Length; i++)
            Debug.Log(mesh.vertices[i].ToString());

        Debug.Log("UVs: " + mesh.uv.Length.ToString());
        for (int i = 0; i < mesh.uv.Length; i++)
            Debug.Log(mesh.uv[i].ToString());

        Debug.Log("Tris: " + mesh.triangles.Length.ToString());
        for (int i = 0; i < mesh.triangles.Length; i++)
            Debug.Log(mesh.triangles[i].ToString());*/

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        return mesh;
    }

    public static Mesh GetMeshFromSprite(Sprite s, float depth)
    {
        Mesh mesh = new Mesh();

        mesh.vertices = GetVerticesFromSprite2(s, depth).ToArray();
        mesh.uv = GetUVsFromSprite2(s).ToArray();
        mesh.triangles = GetTrisFromSprite2(s).ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        return mesh;
    }

    static List<Vector3> GetVerticesFromSprite2(Sprite s, float depth)
    {
        List<Vector3> result = new List<Vector3>();

        // Near Vertices
        foreach (Vector2 v in s.vertices)
        {
            Vector3 near_vertex = (Vector3)v - Vector3.forward * depth * 0.5f;
            result.Add(near_vertex);
        }

        // Far Vertices
        foreach (Vector2 v in s.vertices)
        {
            Vector3 near_vertex = (Vector3)v;
            Vector3 far_vertex = near_vertex + Vector3.forward * depth * 0.5f;

            result.Add(far_vertex);
        }

        return result;
    }

    static List<Vector2> GetUVsFromSprite2(Sprite s)
    {
        List<Vector2> result = new List<Vector2>();

        // Near Vertices
        for (int i = 0; i < s.uv.Length; i++)
        {
            result.Add(s.uv[i]);
        }

        // Far Vertices
        for (int i = 0; i < s.uv.Length; i++)
        {
            result.Add(s.uv[i]);
        }

        return result;
    }

    static List<int> GetTrisFromSprite2(Sprite s)
    {
        List<int> result = new List<int>();

        // Near Face
        for (int i = 0; i < s.triangles.Length; i++)
        {
            result.Add(s.triangles[i]);
        }

        // Far Face
        for (int i = 0; i < s.triangles.Length; i += 3)
        {
            result.Add(s.triangles[i + 2] + s.vertices.Length);
            result.Add(s.triangles[i + 1] + s.vertices.Length);
            result.Add(s.triangles[i] + s.vertices.Length);
        }

        // Sides of mesh
        int[] tris = Array.ConvertAll(s.triangles, i => (int)i);
        List<EdgeHelpers.Edge> edges = EdgeHelpers.GetEdges(tris);
        List<EdgeHelpers.Edge> boundary = EdgeHelpers.FindBoundary(edges).SortEdges();

        for (int i = 0; i < boundary.Count; i++)
        {
            EdgeHelpers.Edge edge = boundary[i];

            int vertex1 = edge.v1; // Near #1
            int vertex2 = edge.v2; // Near #2
            int vertex3 = edge.v1 + s.vertices.Length; // Far #1
            int vertex4 = edge.v2 + s.vertices.Length; // Far #2

            // Triangle 1
            result.Add(vertex1);
            result.Add(vertex3);
            result.Add(vertex2);

            // Triangle 2
            result.Add(vertex2);
            result.Add(vertex3);
            result.Add(vertex4);
        }

        return result;
    }
}

public static class EdgeHelpers
{
    public struct Edge
    {
        public int v1;
        public int v2;
        public int triangleIndex;
        public Edge(int aV1, int aV2, int aIndex)
        {
            v1 = aV1;
            v2 = aV2;
            triangleIndex = aIndex;
        }
    }

    public static List<Edge> GetEdges(int[] aIndices)
    {
        List<Edge> result = new List<Edge>();
        for (int i = 0; i < aIndices.Length; i += 3)
        {
            int v1 = aIndices[i];
            int v2 = aIndices[i + 1];
            int v3 = aIndices[i + 2];
            result.Add(new Edge(v1, v2, i));
            result.Add(new Edge(v2, v3, i));
            result.Add(new Edge(v3, v1, i));
        }
        return result;
    }

    public static List<Edge> FindBoundary(this List<Edge> aEdges)
    {
        List<Edge> result = new List<Edge>(aEdges);
        for (int i = result.Count - 1; i > 0; i--)
        {
            for (int n = i - 1; n >= 0; n--)
            {
                if (result[i].v1 == result[n].v2 && result[i].v2 == result[n].v1)
                {
                    // shared edge so remove both
                    result.RemoveAt(i);
                    result.RemoveAt(n);
                    i--;
                    break;
                }
            }
        }
        return result;
    }
    public static List<Edge> SortEdges(this List<Edge> aEdges)
    {
        List<Edge> result = new List<Edge>(aEdges);
        for (int i = 0; i < result.Count - 2; i++)
        {
            Edge E = result[i];
            for (int n = i + 1; n < result.Count; n++)
            {
                Edge a = result[n];
                if (E.v2 == a.v1)
                {
                    // in this case they are already in order so just continoue with the next one
                    if (n == i + 1)
                        break;
                    // if we found a match, swap them with the next one after "i"
                    result[n] = result[i + 1];
                    result[i + 1] = a;
                    break;
                }
            }
        }
        return result;
    }
}