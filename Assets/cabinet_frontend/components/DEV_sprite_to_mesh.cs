using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DEV_sprite_to_mesh : MonoBehaviour {

    public Sprite[] sprites;
    int sprite_index = 0;

    Sprite s;
    MeshRenderer mr;
    MeshFilter mf;

    public Material icon_mat;

    int version = 1;

    public float depth = 1.0f;

    // Use this for initialization
    void Start () {
        mr = gameObject.AddComponent<MeshRenderer>();
        mf = gameObject.AddComponent<MeshFilter>();

        RefreshMesh(version);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            sprite_index++;
            if (sprite_index >= sprites.Length)
                sprite_index = 0;

            RefreshMesh(version);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (version == 1)
                version = 2;
            else if (version == 2)
                version = 1;

            RefreshMesh(version);
        }
    }

    void RefreshMesh(int version)
    {
        s = sprites[sprite_index];

        if (version == 1)
            Version1();
        else if (version == 2)
            Version2();
    }

    void Version1()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = GetVerticesFromSprite1(s).ToArray();
        mesh.uv = s.uv;
        mesh.triangles = Array.ConvertAll(s.triangles, i => (int)i);

        //Debug.Log(mr);

        mr.material = icon_mat;
        mr.material.mainTexture = s.texture;
        
        mf.mesh = mesh;

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
    }

    void Version2()
    {
        //YMeshUtilities.GetAndApplyMeshFromSprite(s, depth, mr, mf);
        ArborMeshUtilities.GetAndApplyMeshFromTexture(s.texture, depth, mr, mf);
    }

    List<Vector3> GetVerticesFromSprite1(Sprite s)
    {
        List<Vector3> result = new List<Vector3>();

        foreach (Vector2 v in s.vertices)
            result.Add((Vector3)v);

        return result;
    }

    /*List<Vector3> GetVerticesFromSprite2(Sprite s)
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

    List<Vector2> GetUVsFromSprite2(Sprite s)
    {
        List<Vector2> result = new List<Vector2>();

        // Near Vertices
        for(int i = 0; i < s.uv.Length; i++)
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

    List<int> GetTrisFromSprite2(Sprite s)
    {
        List<int> result = new List<int>();

        // Near Face
        for (int i = 0; i < s.triangles.Length; i++)
        {
            result.Add(s.triangles[i]);
        }

        // Far Face
        //for (int i = s.triangles.Length-1; i > -1; i--)
        //for (int i = 0; i < s.triangles.Length; i++)
        for (int i = 0; i < s.triangles.Length; i+= 3)
        {
            result.Add(s.triangles[i+2] + s.vertices.Length);
            result.Add(s.triangles[i+1] + s.vertices.Length);
            result.Add(s.triangles[i] + s.vertices.Length); // TODO: make offset num additional vertices
        }

        // Sides of mesh
        int[] tris = Array.ConvertAll(s.triangles, i => (int)i);
        List<EdgeHelpers.Edge> edges = EdgeHelpers.GetEdges(tris);
        List<EdgeHelpers.Edge> boundary = EdgeHelpers.FindBoundary(edges).SortEdges();

        for(int i = 0; i < boundary.Count; i++)
        {
            //Debug.Log(boundary[i].v1.ToString() + " " + boundary[i].v2.ToString());

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
    }*/
}


