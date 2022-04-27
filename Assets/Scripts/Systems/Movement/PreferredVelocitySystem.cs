using DotsNav.Data;
using DotsNav.LocalAvoidance.Data;
using DotsNav.LocalAvoidance.Systems;
using DotsNav.PathFinding.Data;
using DotsNav.PathFinding.Systems;
using DotsNav.Systems;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(DotsNavSystemGroup))]
[UpdateAfter(typeof(PathFinderSystem))]
[UpdateBefore(typeof(RVOSystem))]
partial class PreferredVelocitySystem : SystemBase
{

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        Entities
            .WithBurst()
            .ForEach((DirectionComponent direction, SpeedData steering, PathQueryComponent query,
                ref Translation translation, ref PreferredVelocityComponent preferredVelocity) =>
            {
                var dist = math.length(query.To - translation.Value);
                var speed = math.min(dist * steering.BrakeSpeed, steering.PreferredSpeed);
                preferredVelocity.Value = direction.Value * speed;
                translation.Value.x += preferredVelocity.Value[0] * deltaTime;
                translation.Value.z += preferredVelocity.Value[1] * deltaTime;
            })
            .ScheduleParallel();
    }
}