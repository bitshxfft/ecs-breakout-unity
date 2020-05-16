using Breakout.Component.Input;
using Breakout.Component.Movement;
using Breakout.Component.Spawn;
using Breakout.Component.Tag;
using Breakout.Config;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Breakout.System.Input
{
	[AlwaysSynchronizeSystem]
	[UpdateBefore(typeof(MoveSystem))]
	public class PaddleInputSystem : JobComponentSystem
	{
		private EndSimulationEntityCommandBufferSystem m_ecbSystem = default;
		private EntityQuery m_ballQuery = default;
		private EntityQuery m_ballRequestQuery = default;

		// ----------------------------------------------------------------------------

		protected override void OnCreate()
		{
			base.OnCreate();

			m_ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
			m_ballQuery = GetEntityQuery(ComponentType.ReadOnly<BallTag>());
			m_ballRequestQuery = GetEntityQuery(ComponentType.ReadOnly<BallSpawnRequest>());
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			EntityCommandBuffer ecb = m_ecbSystem.CreateCommandBuffer();

			int ballCount = m_ballQuery.CalculateEntityCount();
			int ballRequestCount = m_ballRequestQuery.CalculateEntityCount();
			EntityArchetype ballRequestArchetype = EntityManager.CreateArchetype(typeof(BallSpawnRequest));

			Entities
				.ForEach((ref Direction direction, in PaddleInput paddleInputData) =>
				{
					float x = 0;
					x += UnityEngine.Input.GetKey(paddleInputData.m_leftKey) ? -1.0f : 0.0f;
					x += UnityEngine.Input.GetKey(paddleInputData.m_rightKey) ? 1.0f : 0.0f;
					direction.m_direction = new float2(x, 0.0f);

					if (UnityEngine.Input.GetKey(paddleInputData.m_spawnBallKey)
						&& ballCount == 0
						&& ballRequestCount == 0)
					{
						Entity spawnRequest = ecb.CreateEntity(ballRequestArchetype);
						ecb.SetComponent<BallSpawnRequest>(
							spawnRequest,
							new BallSpawnRequest
							{
								m_attachToPaddle = true,
								m_delay = 0.0f,
								m_speed = GameConfig.k_defaultBallSpeed,
								m_direction = new float2(0.0f, 1.0f),
							});
					}
				})
				.Run();

			return inputDeps;
		}
	}
}
