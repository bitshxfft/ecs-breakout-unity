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

	// --------------------------------------------------------------------------------

	protected override void OnCreate()
	{
		base.OnCreate();

		m_ballQuery = GetEntityQuery(ComponentType.ReadOnly<BallTag>());
		m_spawnRequestQuery = GetEntityQuery(ComponentType.ReadWrite<BallSpawnData>());
		m_ballPrefabQuery = GetEntityQuery(ComponentType.ReadOnly<BallPrefabData>());
		m_paddleQuery = GetEntityQuery(ComponentType.ReadOnly<PaddleTag>());

		m_spawnRequestArchetype = EntityManager.CreateArchetype(typeof(BallSpawnData));
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		int ballCount = m_ballQuery.CalculateEntityCount();
		int spawnRequestCount = m_spawnRequestQuery.CalculateEntityCount();

		if (ballCount + spawnRequestCount == 0)
		{
			EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
			Entity ballSpawnRequest = ecb.CreateEntity(m_spawnRequestArchetype);
			BallSpawnData ballSpawnData = new BallSpawnData()
			{
				m_delay = 1.0f,
				m_direction = new float3(0.0f, 0.0f, 0.0f),
				m_position = new float3(0.0f, 0.0f, 0.0f),
				m_spawnState = BallSpawnState.AttachToPaddle,
				m_speed = 0.0f,
			};

			ecb.SetComponent(ballSpawnRequest, ballSpawnData);
			ecb.Playback(EntityManager);
			ecb.Dispose();
		}
		
		if (spawnRequestCount > 0)
		{
			EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
			
			// #SteveD >>> make singleton
			NativeArray<BallPrefabData> ballPrefabData = m_ballPrefabQuery.ToComponentDataArray<BallPrefabData>(Allocator.TempJob);
			NativeArray<Entity> paddles = m_paddleQuery.ToEntityArray(Allocator.TempJob);
			// <<<<<<<<<<<

			float dt = Time.DeltaTime;

			Entities
				.ForEach((Entity ballSpawnRequest, int entityInQueryIndex, ref BallSpawnData spawnData) =>
				{
					spawnData.m_delay -= dt;
					if (spawnData.m_delay <= 0.0f)
					{
						Entity ball = ecb.Instantiate(ballPrefabData[0].m_prefab);
						ecb.AddComponent(ball, new Parent { Value = paddles[0] });
						ecb.AddComponent(ball, new LocalToParent { });
						ecb.SetComponent(ball, new Translation() { Value = new float3(0.0f, 32.0f, 0.0f) }); // #SteveD >>> construct from paddle and ball AABBs
						ecb.DestroyEntity(ballSpawnRequest);
					}
				})
				.Run();

			ballPrefabData.Dispose();
			paddles.Dispose();

			ecb.Playback(EntityManager);
			ecb.Dispose();
		}
		
		return inputDeps;
	}
}
