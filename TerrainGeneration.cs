using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGeneration : MonoBehaviour
{

    public int meshWidth = 50;
    public int meshHeight = 50;

    public int octaves = 25;
    public float frequency = 0.5f;
    public float amplitude = 1.5f;
    public float exp = 2f;

    public float offsetX = 0;
    public float offsetZ = 0;

    public Gradient gradient;

    float minHeight;
    float maxHeight;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    private void Start() {
        offsetX = Random.Range(0f, 99999f);
        offsetZ = Random.Range(0f, 99999f);
    }

    void Update()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        // Create Vertices

        vertices = new Vector3[(meshWidth + 1) * (meshHeight + 1)];

        for (int i = 0, z = 0; z <= meshHeight; z++)
        {
            for (int x = 0; x <= meshWidth; x++)
            {
                float y = GenerateHeight(x, z);
                vertices[i] = new Vector3(x, y, z);

                if (y > maxHeight)
                    maxHeight = y;
                if (y < minHeight)
                    minHeight = y;
    
                i++;
            }
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y -= minHeight;
        }

        // Create Triangles

        triangles = new int[meshWidth * meshHeight * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < meshHeight; z++) {
            for (int x = 0; x < meshWidth; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + meshWidth + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + meshWidth + 1;
                triangles[tris + 5] = vert + meshWidth + 2;

                vert++;
                tris += 6;
            }

            vert++;
        }

        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= meshHeight; z++)
        {
            for (int x = 0; x <= meshWidth; x++)
            {
                float height = Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
        
    }

    void UpdateMesh ()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    float GenerateHeight (int x, int z) {
        float y = 0;
        float sum = 0;

        float xCoord = (float)x / meshWidth * frequency + offsetX;
        float zCoord = (float)z / meshHeight * frequency + offsetZ;

        for (int i = 0; i < octaves; i++)
        {
            y += Mathf.PerlinNoise(i * xCoord, i * zCoord) * amplitude/(i+1)*2;

            sum += amplitude/((i+1)*2);
        }
        
        y = Mathf.Pow(y, exp);

        return y;
    }
}