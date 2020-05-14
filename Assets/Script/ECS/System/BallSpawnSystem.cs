using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BallSpawnSystem : JobComponentSystem
{
	private EntityQuery m_paddleQuery = default;
	EndSimulationEntityCommandBufferSystem m_ecbSystem = default;

	// --------------------------------------------------------------------------------

	protected override void OnCreate()
	{
		base.OnCreate();

		m_paddleQuery = GetEntityQuery(ComponentType.ReadOnly<PaddleTag>());
		m_ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
	}

	protected override void OnStartRunning()
	{
		EntityQuery ballPrefabQuery = GetEntityQuery(ComponentType.ReadOnly<BallPrefab>());
		NativeArray<BallPrefab> ballPrefabData = ballPrefabQuery.ToComponentDataArray<BallPrefab>(Allocator.Temp);
		SetSingleton(ballPrefabData[0]);
		ballPrefabData.Dispose();
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		EntityCommandBuffer.Concurrent ecb = m_ecbSystem.CreateCommandBuffer().ToConcurrent();
		NativeArray<Entity> paddles = m_paddleQuery.ToEntityArray(Allocator.TempJob);
		Entity ballPrefab = GetSingleton<BallPrefab>().m_prefab;
		float dt = Time.DeltaTime;

		JobHandle jobHandle = Entities
			.WithDeallocateOnJobCompletion(paddles)
			.ForEach((Entity ballSpawnRequest, int entityInQueryIndex, ref BallSpawnRequest spawnData) =>
			{
				spawnData.m_delay -= dt;
				if (spawnData.m_delay <= 0.0f)
				{
					Entity ball = ecb.Instantiate(entityInQueryIndex, ballPrefab);
					if (spawnData.m_attachToPaddle)
					{
						ecb.AddComponent(entityInQueryIndex, ball, new Parent { Value = paddles[0] });
						ecb.AddComponent(entityInQueryIndex, ball, new LocalToParent { });
						ecb.AddComponent(entityInQueryIndex, ball, new BlockMovement { });

						// #SD >>> construct from paddle and ball AABBs
						ecb.SetComponent(entityInQueryIndex, ball, new Translation() { Value = new float3(0.0f, 32.0f, 0.0f) });
						// <<<<<<<
					}
					else
					{
						ecb.SetComponent(entityInQueryIndex, ball, new Translation() { Value = new float3(spawnData.m_position.x, spawnData.m_position.y, 0.0f) });
					}

					ecb.SetComponent(entityInQueryIndex, ball, new Direction() { m_direction = spawnData.m_direction });
					ecb.SetComponent(entityInQueryIndex, ball, new Speed() { m_speed = spawnData.m_speed });

					ecb.DestroyEntity(entityInQueryIndex, ballSpawnRequest);
				}
			})
			.Schedule(inputDeps);

		m_ecbSystem.AddJobHandleForProducer(jobHandle);
		
		return jobHandle;
	}
}
