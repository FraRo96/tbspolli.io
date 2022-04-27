using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using DotsNav.Data;
using DotsNav.Hybrid;
using DotsNav.PathFinding.Data;
using static DotsNav.PathFinding.PathQueryState;
using DotsNav.PathFinding.Hybrid;
using DotsNav.Systems;
using UnityEngine;

public partial class PathQuerySystem : SystemBase
{

    protected override void OnUpdate()
    {
        var dirX = 10f;
        var dirZ = 10f;

        Entities.
            ForEach((ref Translation translation, ref PathQueryComponent query, in PlayerTag tag, in Rotation rotation, in DynamicBuffer<PathSegmentElement> path) =>
            {
                query.State = Pending;
                query.To = new float3(dirX, 0, dirZ);
            }).Schedule();
    }
}