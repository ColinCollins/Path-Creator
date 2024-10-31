using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PathCreation.Examples;
using Sirenix.OdinInspector;
using UnityEngine;

// 记录顶点
public class Node
{
    public int nextIndex;
    public int selfIndex;
    public Vector3[] vertex;

    public int[] Bottom = new int[] { 0, 1, 2, 3 };
}

public partial class DynamicMeshGenerator
{
    private Vector3[] initPoint;

    public float Speed = 1f;

    private List<Node> nodes;

    [Button("重置")]
    public void Stop()
    {
        verts = initPoint;
        isPlaying = false;
    }

    private bool isPlaying = false;

    [Button("播放")]
    public void Play()
    {
        isPlaying = true;

        index = _points.Length - 1;
        lastPoint = _points.Last();
        
        if (nodes == null)
            nodes = new List<Node>();

        nodes.Clear();
        Node node;
        for (int i = 0; i < path.NumPoints; i++)
        {
            node = new Node();
            node.selfIndex = i;
            node.nextIndex = i + 1 >= path.NumPoints ? i : i + 1;
            node.vertex = new Vector3[8];

            for (int j = 0; j < 8; j++)
            {
                node.vertex[j] = initPoint[i * 8 + j];
            }

            nodes.Add(node);
        }
    }

    private bool CheckFinished()
    {
        if (nodes == null || nodes.Count <= 0)
            return true;
        
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].nextIndex != nodes.Count - 1)
            {
                return false;
            }
        }

        return true;
    }

    private int index = 0;
    private Transform lastPoint;
    void Update()
    {
        MoveMesh();
    }

    void MoveMesh()
    {
        bool isFinished = CheckFinished();

        if (!isPlaying ||
            initPoint == null ||
            initPoint.Length <= 0 ||
            isFinished)
            return;

        int vertIndex = 0;

        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            if (node.nextIndex + 1 >= nodes.Count)
                continue;

            for (int j = 0; j < 8; j++)
            {
                var startPos = node.vertex[j];
                var nextPos = initPoint[node.nextIndex * 8 + j];
                if (Vector3.Distance(startPos, nextPos) <= 0.001f)
                {
                    node.nextIndex++;
                }

                node.vertex[j] = Vector3.MoveTowards(startPos, nextPos, Speed * Time.deltaTime);
                verts[node.selfIndex * 8 + j] = node.vertex[j];
            }

            vertIndex += 8;
        }

        RefreshMesh();
    }
}