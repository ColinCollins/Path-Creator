using System.Collections;
using System.Collections.Generic;
using PathCreation;
using PathCreation.Examples;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 依托 road Mesh Creator 的简化版本
/// </summary>
public partial class DynamicMeshGenerator : PathSceneTool
{
    private Transform[] _points;

    [Header("Road settings")] public float roadWidth = .4f;
    [Range(0, 1.5f)] public float thickness = .15f;
    public bool flattenSurface;

    [Header("Material settings")] public Material roadMaterial;
    // public Material undersideMaterial;
    public float textureTiling = 1;

    [SerializeField, HideInInspector] GameObject meshHolder;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Mesh mesh;

    public Vector3[] verts;
    public Vector2[] uvs;
    public Vector3[] normals;
    
    public int[] roadTriangles;
    public int[] underRoadTriangles;
    public int[] sideOfRoadTriangles;

    public void SetPoints(Transform[] points)
    {
        _points = points;
        BezierPath _bezierPath = new BezierPath (_points, false, PathSpace.xyz);
        pathCreator.bezierPath = _bezierPath;
    }

    /// <summary>
    /// 生成 mesh
    /// </summary>
    public void GenerateMesh()
    {
        if (_points == null || _points.Length <= 0)
            return;
    }


    /// <summary>
    /// 刷新 mesh
    /// </summary>
    public void RefreshMesh()
    {
        mesh.Clear();
        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.subMeshCount = 3;
        mesh.SetTriangles (roadTriangles, 0);
        mesh.SetTriangles (underRoadTriangles, 1);
        mesh.SetTriangles (sideOfRoadTriangles, 2);
        mesh.RecalculateBounds();
        // mesh.RecalculateNormals();
    }

    protected override void PathUpdated()
    {
        if (pathCreator != null)
        {
            AssignMeshComponents();
            AssignMaterials();
            CreateRoadMesh();
        }
    }

    void CreateRoadMesh()
    {
        verts = new Vector3[path.NumPoints * 8];
        uvs = new Vector2[verts.Length];
        normals = new Vector3[verts.Length];

        int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
        roadTriangles = new int[numTris * 3];
        underRoadTriangles = new int[numTris * 3];
        sideOfRoadTriangles = new int[numTris * 2 * 3];


        int vertIndex = 0;
        int triIndex = 0;

        // Vertices for the top of the road are layed out:
        // 0  1
        // 8  9
        // and so on... So the triangle map 0,8,1 for example,
        // defines a triangle from top left to bottom left to bottom right.
        int[] triangleMap = { 0, 8, 1, 1, 8, 9 };
        int[] sidesTriangleMap = { 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5 };

        bool usePathNormals = !(path.space == PathSpace.xyz && flattenSurface);

        for (int i = 0; i < path.NumPoints; i++)
        {
            Vector3 localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : path.up;
            Vector3 localRight = (usePathNormals) ? path.GetNormal(i) : Vector3.Cross(localUp, path.GetTangent(i));

            // Find position to left and right of current path vertex
            Vector3 vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(roadWidth);
            Vector3 vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(roadWidth);

            // Add top of road vertices
            verts[vertIndex + 0] = vertSideA;
            verts[vertIndex + 1] = vertSideB;
            // Add bottom of road vertices, double thickness move0
            verts[vertIndex + 2] = vertSideA - localUp * thickness;
            verts[vertIndex + 3] = vertSideB - localUp * thickness;

            // Duplicate vertices to get flat shading for sides of road
            verts[vertIndex + 4] = verts[vertIndex + 0];
            verts[vertIndex + 5] = verts[vertIndex + 1];
            verts[vertIndex + 6] = verts[vertIndex + 2];
            verts[vertIndex + 7] = verts[vertIndex + 3];

            // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
            uvs[vertIndex + 0] = new Vector2(0, path.times[i] * 10f);
            uvs[vertIndex + 1] = new Vector2(1, path.times[i] * 10f);
            uvs[vertIndex + 2] = new Vector2(0, path.times[i] * 10f);
            uvs[vertIndex + 3] = new Vector2(1, path.times[i] * 10f);
            uvs[vertIndex + 4] = new Vector2(0, path.times[i] * 10f);
            uvs[vertIndex + 5] = new Vector2(1, path.times[i] * 10f);
            uvs[vertIndex + 7] = new Vector2(0, path.times[i] * 10f);
            uvs[vertIndex + 6] = new Vector2(1, path.times[i] * 10f);

            // Top of road normals
            normals[vertIndex + 0] = (localUp - localRight).normalized;
            normals[vertIndex + 1] = (localUp + localRight).normalized;
            // Bottom of road normals
            normals[vertIndex + 2] = (-localUp - localRight).normalized;
            normals[vertIndex + 3] = (-localUp + localRight).normalized;
            // Sides of road normals
            normals[vertIndex + 4] = -localRight;
            normals[vertIndex + 5] = localRight;
            normals[vertIndex + 6] = -localRight;
            normals[vertIndex + 7] = localRight;

            // Set triangle indices
            if (i < path.NumPoints - 1 || path.isClosedLoop) {
                for (int j = 0; j < triangleMap.Length; j++) {
                    roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                    // reverse triangle map for under road so that triangles wind the other way and are visible from underneath
                    underRoadTriangles[triIndex + j] = (vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2) % verts.Length;
                }
                for (int j = 0; j < sidesTriangleMap.Length; j++) {
                    sideOfRoadTriangles[triIndex * 2 + j] = (vertIndex + sidesTriangleMap[j]) % verts.Length;
                }

            }

            vertIndex += 8;
            triIndex += 6;
        }

        RefreshMesh();

        // save
        initPoint = new Vector3[verts.Length];
        for (int i = 0; i < verts.Length; i++)
        {
            initPoint[i] = verts[i];
        }
    }

    void AssignMeshComponents()
    {
        if (meshHolder == null)
        {
            meshHolder = new GameObject("Road Mesh Holder");
        }

        meshHolder.transform.rotation = Quaternion.identity;
        meshHolder.transform.position = Vector3.zero;
        meshHolder.transform.localScale = Vector3.one;

        // Ensure mesh renderer and filter components are assigned
        if (!meshHolder.gameObject.GetComponent<MeshFilter>())
        {
            meshHolder.gameObject.AddComponent<MeshFilter>();
        }

        if (!meshHolder.GetComponent<MeshRenderer>())
        {
            meshHolder.gameObject.AddComponent<MeshRenderer>();
        }

        meshRenderer = meshHolder.GetComponent<MeshRenderer>();
        meshFilter = meshHolder.GetComponent<MeshFilter>();
        if (mesh == null)
        {
            mesh = new Mesh();
        }

        meshFilter.sharedMesh = mesh;
    }

    void AssignMaterials()
    {
        meshRenderer.sharedMaterials = new Material[] { roadMaterial, roadMaterial, roadMaterial };
        /*if (roadMaterial != null && undersideMaterial != null)
        {
            meshRenderer.sharedMaterials = new Material[] { roadMaterial, undersideMaterial, undersideMaterial };
            meshRenderer.sharedMaterials[0].mainTextureScale = new Vector3(1, textureTiling);
        }*/
    }
}