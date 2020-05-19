using Breakout.Component.Brick;
using Breakout.Component.Powerup;
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
		
		// ----------------------------------------------------------------------------

		protected override void OnCreate()
		{
			base.OnCreate();

			m_ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var ecb = m_ecbSystem.CreateCommandBuffer().ToConcurrent();

			JobHandle jobHandle = Entities
				.WithAll<BrickTag>()
				.ForEach((Entity entity, int entityInQueryIndex, in Level level, in Translation translation) =>
				{
					if (level.m_level == 0)
					{
						ecb.DestroyEntity(entityInQueryIndex, entity);

						// #SteveD >>> acquire random outside of ForEach
						//Random random = new Random(0);
						float r = 0.25f;// random.NextFloat(0.0f, 1.0f);
						if (r < GameConfig.k_multiball10Value)
						{
							Entity powerupRequest = ecb.CreateEntity(entityInQueryIndex);
							Powerup powerup = new Powerup { m_powerup = PowerupType.None, m_context = 0.0f };

							if (r <= GameConfig.k_paddleSpeedUpValue)
							{
								powerup = new Powerup { m_powerup = PowerupType.PaddleSpeed, m_context = GameConfig.k_paddleSpeedMultiplier };
							}
							else if (r <= GameConfig.k_ballSpeedUpValue)
							{
								powerup = new Powerup { m_powerup = PowerupType.BallSpeed, m_context = GameConfig.k_ballSpeedMultiplier };
							}
							else if (r <= GameConfig.k_ballSpeedDownValue)
							{
								powerup = new Powerup { m_powerup = PowerupType.BallSpeed, m_context = 1.0f / GameConfig.k_ballSpeedMultiplier };
							}
							else if (r <= GameConfig.k_paddleSizeUpValue)
							{
								powerup = new Powerup { m_powerup = PowerupType.PaddleSize, m_context = GameConfig.k_paddleSizeMultiplier };
							}
							else if (r <= GameConfig.k_paddleSizeDownValue)
							{
								powerup = new Powerup { m_powerup = PowerupType.PaddleSize, m_context = 1.0f / GameConfig.k_paddleSizeMultiplier };
							}
							else if (r <= GameConfig.k_multiball1Value)
							{
								powerup = new Powerup { m_powerup = PowerupType.Multiball, m_context = 1.0f };
							}
							else if (r <= GameConfig.k_multiball2Value)
							{
								powerup = new Powerup { m_powerup = PowerupType.Multiball, m_context = 2.0f };
							}
							else if (r <= GameConfig.k_multiball3Value)
							{
								powerup = new Powerup { m_powerup = PowerupType.Multiball, m_context = 3.0f };
							}
							else if (r <= GameConfig.k_multiball5Value)
							{
								powerup = new Powerup { m_powerup = PowerupType.Multiball, m_context = 5.0f };
							}
							else if (r <= GameConfig.k_multiball10Value)
							{
								powerup = new Powerup { m_powerup = PowerupType.Multiball, m_context = 10.0f };
							}

							ecb.AddComponent(entityInQueryIndex, powerupRequest, 
								new PowerupSpawnRequest 
								{ 
									m_powerup = powerup, 
									m_position = new float2(translation.Value.x, translation.Value.y)
								});
						}
					}
				})
				.Schedule(inputDeps);

			m_ecbSystem.AddJobHandleForProducer(jobHandle);
			return jobHandle;
		}
	}
}
