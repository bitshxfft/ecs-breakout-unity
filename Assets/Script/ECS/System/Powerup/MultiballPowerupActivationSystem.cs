using Breakout.Component.Ball;
using Breakout.Component.Powerup;
using Breakout.Component.Tag;
using Breakout.Config;
using Breakout.System.Spawn;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Breakout.System.Powerup
{
	[UpdateAfter(typeof(PowerupSpawnSystem))]
	public sealed class MultiballPowerupActivationSystem : PowerupActivationSystem<MultiballPowerupActivationRequest>
	{
		private EntityQuery m_paddleQuery = default;

		// --------------------------------------------------------------------------------

		protected override void OnCreate()
		{
			base.OnCreate();

			m_paddleQuery = GetEntityQuery(
				ComponentType.ReadWrite<PaddleTag>(),
				ComponentType.ReadWrite<Translation>());
		}
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			EntityCommandBuffer.Concurrent ecb = m_ecbSystem.CreateCommandBuffer().ToConcurrent();
			var paddleTranslations = m_paddleQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

			JobHandle jobHandle = Entities
				.WithDeallocateOnJobCompletion(paddleTranslations)
				.ForEach((Entity entity, int entityInQueryIndex, in MultiballPowerupActivationRequest request) =>
				{
					float angleStep = math.radians(GameConfig.k_maxPaddleBallReflectAngle * 2) / request.m_ballCount;
					float halfAngleStep = angleStep * 0.5f;
					float angle = math.radians(-GameConfig.k_maxPaddleBallReflectAngle) + halfAngleStep;
					
					for (int p = 0; p < paddleTranslations.Length; ++p)
					{
						for (int b = 0; b < request.m_ballCount; ++b)
						{
							Entity spawnRequest = ecb.CreateEntity(entityInQueryIndex);
							ecb.AddComponent(entityInQueryIndex, spawnRequest,
								new BallSpawnRequest
								{
									m_attachToPaddle = false,
									m_delay = 0.0f,
									m_speed = GameConfig.k_defaultBallSpeed,
									m_direction = new float2(-math.sin(angle), math.cos(angle)),
									m_position = new float2(paddleTranslations[p].Value.x, paddleTranslations[p].Value.y + 32.0f),
								});

							angle += angleStep;
						}
					}

					ecb.DestroyEntity(entityInQueryIndex, entity);
				})
				.Schedule(inputDeps);

			m_ecbSystem.AddJobHandleForProducer(jobHandle);
			return jobHandle;
		}
	}
}
