using Breakout.Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

// #SD >>> request ball from GameManager (Access EntityManager from DefaultWorld?)

namespace Breakout.System
{
	public class BallRequestSystem : JobComponentSystem
	{
		private EntityQuery m_ballQuery = default;
		private EntityQuery m_spawnRequestQuery = default;
		private EntityArchetype m_spawnRequestArchetype = default;
		
		// --------------------------------------------------------------------------------

		protected override void OnCreate()
		{
			base.OnCreate();

			m_ballQuery = GetEntityQuery(ComponentType.ReadOnly<BallTag>());
			m_spawnRequestQuery = GetEntityQuery(ComponentType.ReadWrite<BallSpawnRequest>());
			m_spawnRequestArchetype = EntityManager.CreateArchetype(typeof(BallSpawnRequest));
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
				BallSpawnRequest ballSpawnData = new BallSpawnRequest()
				{
					m_delay = 0.25f,
					m_direction = new float2(0.0f, 1.0f),
					m_position = new float2(0.0f, 0.0f),
					m_attachToPaddle = true,
					// #SD >>> defer to default speed (if not set?)
					m_speed = 1080.0f,
					// <<<<<<<
				};

				ecb.SetComponent(ballSpawnRequest, ballSpawnData);
				ecb.Playback(EntityManager);
				ecb.Dispose();
			}
			
			return jobHandle;
		}
	}
}
