using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;


public class Voxel
{
    int x, y, z;

    void CreateShape(Vector3[] verticies, int[] triangles)
    {
        
    }
}

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    

    Vector3[] heightMap;
    Vector3[] verticies;
    int[] triangles;
    Color[] colors;

    public int xSize = 20;
    public int zSize = 20;

    public float strength = .3f;
    public float maxHeight = 2f;
    public Gradient gradient;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateMesh();
        UpdateMesh();
    }

    void CreateMesh()
    {
        verticies = new Vector3[(xSize + 1) * (zSize + 1)];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * strength, z * strength) * maxHeight;
                y += Mathf.PerlinNoise(x * strength*5, z * strength*5) * maxHeight/8;
                y += Mathf.PerlinNoise(x * strength*25, z * strength*20) * maxHeight/40;

                verticies[i] = new Vector3(x, Mathf.Floor(y), z);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
                
                vert++;
                tris += 6;
            }
            vert++;
        }

        colors = new Color[verticies.Length];
        for (int i = 0; i < verticies.Length; i++)
        {
            float height = verticies[i].y;
            colors[i] = gradient.Evaluate(height / maxHeight);
        }

    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

    private void OnRenderObject()
    { 
        CreateMesh();
        UpdateMesh();
    }

    private void OnDrawGizmos()
    {
        if (verticies == null) { return; }

        for (int i = 0; i < verticies.Length; i++)
        {
            Gizmos.DrawSphere(verticies[i], .1f);
        }
    }

    void MarchingSquareLUT(int state, int[] triangles, int tris, int vert)
    {
        switch(state)
        {
            case 0:
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
                break;
        }
    }
}
