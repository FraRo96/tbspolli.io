using Unity.Entities;
using Unity.Collections;
using System.Collections.Generic;
using System.Collections;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public static class Edges
{
    public const float leftX = -45f;
    public const float rightX = 45f;
    public const float downZ = -45f;
    public const float upZ = 45f;
}

public class EntitiesSpawner : MonoBehaviour
{
    [SerializeField]
    public GameObject AllyPrefab;
    [SerializeField]
    public GameObject EnemyPrefab;
    public int NumAllies = 20;
    public int NumEnemies = 4;
    public float AllyRadius = 1.5f;
    public float DistancePlayerEnemy = 6f;
    public float EnemyRadius = 1.5f;
    public float TimeForEnemies = 5f;


    private bool _playerLiving = true;

    void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        var settings = GameObjectConversionSettings.FromWorld(world, null);
        var allyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(AllyPrefab, settings);
        var enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(EnemyPrefab, settings);
        var entityManager = world.EntityManager;

        NativeArray<Entity> allyEntities = entityManager.Instantiate(allyEntityPrefab, NumAllies, Allocator.Temp);
        StartCoroutine(SpawnRoutine(enemyEntityPrefab, entityManager)); //race condition if a coroutine for enemies and one for allies would run in parallel?



        NoOverlapPositioning(allyEntities, entityManager, AllyRadius);
    }

    IEnumerator SpawnRoutine(Entity enemyEntityPrefab, EntityManager entityManager)
    {
        float3 position = float3.zero;
        float dist;


        for (int hordeNum = 0; hordeNum < 2 && _playerLiving; hordeNum++)
        {
            NativeArray<Entity> enemyEntities = entityManager.Instantiate(enemyEntityPrefab, NumEnemies, Allocator.Temp);
            do
            {
                position = new float3(UnityEngine.Random.Range(Edges.leftX, Edges.rightX), 1, UnityEngine.Random.Range(Edges.downZ, Edges.upZ));
                dist = Vector3.Distance(position, GameObject.Find("Player").transform.position);
            }

            while (dist < DistancePlayerEnemy // enemies are spawned at least at some distance from player
                || NoOverlapPositioning(enemyEntities, entityManager, EnemyRadius, position) == 0); // at least 1 enemy is positioned

            yield return new WaitForSeconds(TimeForEnemies);
        }
    }

    /* returns the number of positioned entities */
    private int NoOverlapPositioning(NativeArray<Entity> entities, EntityManager entityManager, float radius, params Vector3[] mainPosition)
    {
        bool idle = false;
        int nTries = 0;
        int maxTries = 100;
        Collider[] overlaps = null;
        float3 position = float3.zero;
        int positioned = 0;

        foreach (var entity in entities)
        {
            idle = false;

            while (!idle && nTries < maxTries)
            {
                if (mainPosition.Length == 0) // instantiating Allies
                {
                    position = new float3(UnityEngine.Random.Range(Edges.leftX, Edges.rightX), 1, UnityEngine.Random.Range(Edges.downZ, Edges.upZ));
                }

                else // instantiating randomly a group of Enemies (of length NumEnemies) around (5 max dist for each coord) mainPosition
                {
                    position = new float3(UnityEngine.Random.Range(-5f, 5f) + mainPosition[0].x, 1, UnityEngine.Random.Range(-5f, 5f) + mainPosition[0].z);
                }

                overlaps = Physics.OverlapSphere(position, radius);

                if (overlaps.Length == 0 || (overlaps.Length == 1 && overlaps[0].gameObject.tag == "WalkingScene"))
                {
                    entityManager.SetComponentData(entity, new Translation
                    {
                        Value = position
                    });
                    idle = true;
                    positioned += 1;
                }

                else
                {
                    nTries += 1;
                }
            }
        }

        return positioned;
    }
}