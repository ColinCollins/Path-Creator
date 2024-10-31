using System.Collections;
using System.Collections.Generic;
using PathCreation;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(PathCreator))]
public class SetPointsTest : MonoBehaviour
{
    public Transform[] waypoints;
    
    [Button("测试生成网格")]
    public void SetPoints()
    {
        if (waypoints.Length > 0) {
            GetComponent<DynamicMeshGenerator>()?.SetPoints(waypoints);
        }
    }

    [Button("播放动画")]
    public void PlayAnimation()
    {
        GetComponent<DynamicMeshGenerator>()?.Play();
    }
    
    [Button("停止播放")]
    public void StopAnimation()
    {
        GetComponent<DynamicMeshGenerator>()?.Stop();
    }
}
