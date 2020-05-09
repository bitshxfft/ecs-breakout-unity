using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BallSpawnSystem : JobComponentSystem
{
	private EntityQuery m_ballQuery = default;
	private EntityQuery m_spawnRequestQuery = default;
	private EntityQuery m_ballPrefabQuery = default;
	private EntityQuery m_paddleQuery = default;

	private EntityArchetype m_spawnRequestArchetype = default;

	EndSimulationEntityCommandBufferSystem m_ecbSystem = default;

	// --------------------------------------------------------------------------------

	protected override void OnCreate()
	{
		base.OnCreate();

		m_ballQuery = GetEntityQuery(ComponentType.ReadOnly<BallTag>());
		m_spawnRequestQuery = GetEntityQuery(ComponentType.ReadWrite<BallSpawnData>());
		m_ballPrefabQuery = GetEntityQuery(ComponentType.ReadOnly<BallPrefabData>());
		m_paddleQuery = GetEntityQuery(ComponentType.ReadOnly<PaddleTag>());

		m_spawnRequestArchetype = EntityManager.CreateArchetype(typeof(BallSpawnData));

		m_ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		JobHandle jobHandle = inputDeps;

		int ballCount = m_ballQuery.CalculateEntityCount();
		int spawnRequestCount = m_spawnRequestQuery.CalculateEntityCount();

		if (ballCount + spawnRequestCount == 0)
		{
			EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
			Entity ballSpawnRequest = ecb.CreateEntity(m_spawnRequestArchetype);
			BallSpawnData ballSpawnData = new BallSpawnData()
			{
				m_delay = 0.25f,
				m_direction = new float3(0.0f, 1.0f, 0.0f),
				m_position = new float3(0.0f, 0.0f, 0.0f),
				m_spawnState = BallSpawnState.AttachToPaddle,
				m_speed = 1080.0f, // #SD-TODO >>> pull from component/static (default speed)
			};

			ecb.SetComponent(ballSpawnRequest, ballSpawnData);
			ecb.Playback(EntityManager);
			ecb.Dispose();
		}
		else if (spawnRequestCount > 0)
		{
			EntityCommandBuffer.Concurrent ecb = m_ecbSystem.CreateCommandBuffer().ToConcurrent();

			// #SD-TODO >>> don't want to get these every time!
			NativeArray<BallPrefabData> ballPrefabData = m_ballPrefabQuery.ToComponentDataArray<BallPrefabData>(Allocator.TempJob);
			NativeArray<Entity> paddles = m_paddleQuery.ToEntityArray(Allocator.TempJob);
			// <<<<<<<<<<<

			float dt = Time.DeltaTime;

			jobHandle = Entities
				.WithDeallocateOnJobCompletion(ballPrefabData)
				.WithDeallocateOnJobCompletion(paddles)
				.ForEach((Entity ballSpawnRequest, int entityInQueryIndex, ref BallSpawnData spawnData) =>
				{
					spawnData.m_delay -= dt;
					if (spawnData.m_delay <= 0.0f)
					{
						Entity ball = ecb.Instantiate(entityInQueryIndex, ballPrefabData[0].m_prefab);
						ecb.AddComponent(entityInQueryIndex, ball, new Parent { Value = paddles[0] });
						ecb.AddComponent(entityInQueryIndex, ball, new LocalToParent { });
						ecb.SetComponent(entityInQueryIndex, ball, new MoveData() { m_direction = spawnData.m_direction, m_speed = spawnData.m_speed });

						// #SteveD >>> construct from paddle and ball AABBs
						ecb.SetComponent(entityInQueryIndex, ball, new Translation() { Value = new float3(0.0f, 32.0f, 0.0f) });
						// <<<<<<<<<<<

						ecb.DestroyEntity(entityInQueryIndex, ballSpawnRequest);
					}
				})
				.Schedule(inputDeps);

			m_ecbSystem.AddJobHandleForProducer(jobHandle);
		}
		
		return jobHandle;
	}
}
