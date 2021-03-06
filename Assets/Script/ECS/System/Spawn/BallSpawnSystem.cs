﻿using Breakout.Component.Ball;
using Breakout.Component.Movement;
using Breakout.Component.Prefab;
using Breakout.Component.Tag;
using Breakout.Config;
using Breakout.System.Collision;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Breakout.System.Spawn
{
	[UpdateAfter(typeof(BallCollisionResolutionSystem))]
	public class BallSpawnSystem : JobComponentSystem
	{
		private EndSimulationEntityCommandBufferSystem m_ecbSystem = default;
		private EntityQuery m_paddleQuery = default;
		private EntityQuery m_ballPrefabQuery = default;

		// --------------------------------------------------------------------------------

		protected override void OnCreate()
		{
			base.OnCreate();

			m_ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
			m_paddleQuery = GetEntityQuery(ComponentType.ReadOnly<PaddleTag>());
			m_ballPrefabQuery = GetEntityQuery(ComponentType.ReadOnly<BallPrefab>());
			
			RequestBall();
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			EntityCommandBuffer.Concurrent ecb = m_ecbSystem.CreateCommandBuffer().ToConcurrent();
			NativeArray<Entity> paddles = m_paddleQuery.ToEntityArray(Allocator.TempJob);
			NativeArray<BallPrefab> ballPrefabs = m_ballPrefabQuery.ToComponentDataArray<BallPrefab>(Allocator.TempJob);
			
			float dt = Time.DeltaTime;

			JobHandle jobHandle = Entities
				.WithDeallocateOnJobCompletion(paddles)
				.WithDeallocateOnJobCompletion(ballPrefabs)
				.ForEach((Entity entity, int entityInQueryIndex, ref BallSpawnRequest spawnData) =>
				{
					spawnData.m_delay -= dt;
					if (spawnData.m_delay <= 0.0f)
					{
						Entity ballEntity = ecb.Instantiate(entityInQueryIndex, ballPrefabs[0].m_prefab);
						if (spawnData.m_attachToPaddle)
						{
							ecb.AddComponent(entityInQueryIndex, ballEntity, new Parent { Value = paddles[0] });
							ecb.AddComponent(entityInQueryIndex, ballEntity, new LocalToParent { });
							ecb.AddComponent(entityInQueryIndex, ballEntity, new BlockMovement { });
							ecb.SetComponent(entityInQueryIndex, ballEntity, new Translation() { Value = new float3(0.0f, 32.0f, 0.0f) });
						}
						else
						{
							ecb.SetComponent(entityInQueryIndex, ballEntity, new Translation() { Value = new float3(spawnData.m_position.x, spawnData.m_position.y, 0.0f) });
						}

						ecb.SetComponent(entityInQueryIndex, ballEntity, new Direction() { m_direction = spawnData.m_direction });
						ecb.SetComponent(entityInQueryIndex, ballEntity, new Speed() { m_speed = spawnData.m_speed });

						ecb.DestroyEntity(entityInQueryIndex, entity);
					}
				})
				.Schedule(inputDeps);

			m_ecbSystem.AddJobHandleForProducer(jobHandle);
			return jobHandle;
		}

		private void RequestBall()
		{
			Entity spawnRequest = EntityManager.CreateEntity();
			EntityManager.AddComponentData(
				spawnRequest,
				new BallSpawnRequest
				{
					m_attachToPaddle = true,
					m_delay = GameConfig.k_newBallDelay,
					m_speed = GameConfig.k_defaultBallSpeed,
					m_direction = new float2(0.0f, 1.0f),
				});
		}
	}
}
