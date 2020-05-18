using Breakout.Component.Brick;
using Breakout.Component.Powerup;
using Breakout.Component.Spawn;
using Breakout.Component.Tag;
using Breakout.Config;
using Breakout.System.Collision;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Breakout.System.Unspawn
{
	[UpdateAfter(typeof(BrickCollisionResolutionSystem))]
	public class BrickUnspawnSystem : JobComponentSystem
	{
		private EndSimulationEntityCommandBufferSystem m_ecbSystem = default;
		private EntityArchetype m_powerupRequestArchetype = default;

		// ----------------------------------------------------------------------------

		protected override void OnCreate()
		{
			base.OnCreate();

			m_ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
			m_powerupRequestArchetype = EntityManager.CreateArchetype(typeof(PowerupSpawnRequest));
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var ecb = m_ecbSystem.CreateCommandBuffer().ToConcurrent();
			EntityArchetype powerupRequestArchetype = m_powerupRequestArchetype;

			JobHandle jobHandle = Entities
				.WithAll<BrickTag>()
				.ForEach((Entity entity, int entityInQueryIndex, in Level level, in Translation translation) =>
				{
					if (level.m_level == 0)
					{
						ecb.DestroyEntity(entityInQueryIndex, entity);

						Random random = new Random();
						float r = random.NextFloat(0.0f, 1.0f);
						if (r < GameConfig.k_multiball10Value)
						{
							Entity powerupRequest = ecb.CreateEntity(entityInQueryIndex, powerupRequestArchetype);
							ecb.SetComponent(entityInQueryIndex, entity, new Translation { Value = translation.Value });

							if (r <= GameConfig.k_paddleSpeedUpValue)
							{
								ecb.SetComponent(entityInQueryIndex, entity, new PowerupSpawnRequest { m_powerup = PowerupType.PaddleSpeedUp });
							}
							else if (r <= GameConfig.k_ballSpeedUpValue)
							{
								ecb.SetComponent(entityInQueryIndex, entity, new PowerupSpawnRequest { m_powerup = PowerupType.BallSpeedUp });
							}
							else if (r <= GameConfig.k_ballSpeedDownValue)
							{
								ecb.SetComponent(entityInQueryIndex, entity, new PowerupSpawnRequest { m_powerup = PowerupType.BallSpeedDown });
							}
							else if (r <= GameConfig.k_paddleSizeUpValue)
							{
								ecb.SetComponent(entityInQueryIndex, entity, new PowerupSpawnRequest { m_powerup = PowerupType.PaddleSizeUp });
							}
							else if (r <= GameConfig.k_paddleSizeDownValue)
							{
								ecb.SetComponent(entityInQueryIndex, entity, new PowerupSpawnRequest { m_powerup = PowerupType.PaddleSizeDown });
							}
							else if (r <= GameConfig.k_multiball1Value)
							{
								ecb.SetComponent(entityInQueryIndex, entity, new PowerupSpawnRequest { m_powerup = PowerupType.Multiball_1 });
							}
							else if (r <= GameConfig.k_multiball2Value)
							{
								ecb.SetComponent(entityInQueryIndex, entity, new PowerupSpawnRequest { m_powerup = PowerupType.Multiball_2 });
							}
							else if (r <= GameConfig.k_multiball3Value)
							{
								ecb.SetComponent(entityInQueryIndex, entity, new PowerupSpawnRequest { m_powerup = PowerupType.Multiball_3 });
							}
							else if (r <= GameConfig.k_multiball5Value)
							{
								ecb.SetComponent(entityInQueryIndex, entity, new PowerupSpawnRequest { m_powerup = PowerupType.Multiball_5 });
							}
							else if (r <= GameConfig.k_multiball10Value)
							{
								ecb.SetComponent(entityInQueryIndex, entity, new PowerupSpawnRequest { m_powerup = PowerupType.Multiball_10 });
							}
						}
					}
				})
				.Schedule(inputDeps);

			m_ecbSystem.AddJobHandleForProducer(jobHandle);
			return jobHandle;
		}
	}
}
