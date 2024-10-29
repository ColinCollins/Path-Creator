using System.Collections;
using System.Collections.Generic;
using PathCreation.Examples;
using UnityEngine;

/// <summary>
/// 依托 road Mesh Creator 的简化版本
/// </summary>
public class DynamicMeshGenerator : PathSceneTool
{
    private Vector3[] _points;

    [Header ("Road settings")]
    public float roadWidth = .4f;
    [Range (0, 1.5f)]
    public float thickness = .15f;
    public bool flattenSurface;

    [Header ("Material settings")]
    public Material roadMaterial;
    public Material undersideMaterial;
    public float textureTiling = 1;

    [SerializeField, HideInInspector]
    GameObject meshHolder;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Mesh mesh;
    
    public void SetPoints(Vector3[] points)
    {
        _points = points;
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
        
    }

    protected override void PathUpdated()
    {
        if (pathCreator != null) {
            // AssignMeshComponents ();
            // AssignMaterials ();
            // CreateRoadMesh ();
        }
    }
}
