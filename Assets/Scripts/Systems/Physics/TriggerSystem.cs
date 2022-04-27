/*using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(StepPhysicsWorld))]
[UpdateBefore(typeof(ExportPhysicsWorld))]


public partial class TriggerSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem commandBufferSystem;
    private StepPhysicsWorld stepPhysicsWorld;
    protected override void OnStartRunning()
    {
        this.RegisterPhysicsRuntimeSystemReadOnly();
    }

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override void OnUpdate()
    {
        var triggerJob = new TriggerJob
        {
            allEnemies = GetComponentDataFromEntity<EnemyTag>(true),
            player = GetEntityQuery(ComponentType.ReadOnly<PlayerTag>()).GetSingletonEntity(),
            entityCommandBuffer = commandBufferSystem.CreateCommandBuffer(),
            
    };

        Dependency = triggerJob.Schedule(stepPhysicsWorld.Simulation, Dependency);
        commandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}


[BurstCompile]
struct TriggerJob : ITriggerEventsJob
{
    [ReadOnly] public ComponentDataFromEntity<EnemyTag> allEnemies;
    public Entity player;
    public EntityCommandBuffer entityCommandBuffer;

    public void Execute(TriggerEvent triggerEvent)
    {
        Entity entityA = triggerEvent.EntityA;
        Entity entityB = triggerEvent.EntityB;

        if (allEnemies.HasComponent(entityA) && allEnemies.HasComponent(entityB)) return;

        if (allEnemies.HasComponent(entityA) && player == entityB)
        {
            entityCommandBuffer.DestroyEntity(entityA);
            
        }
        else if (allEnemies.HasComponent(entityB) && player == entityA)
        {
            entityCommandBuffer.DestroyEntity(entityB);
        }
    }
}*/