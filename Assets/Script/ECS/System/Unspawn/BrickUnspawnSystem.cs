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
		private Random m_random = default;

		// ----------------------------------------------------------------------------

		protected override void OnCreate()
		{
			base.OnCreate();

			m_ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
			
			m_random = new Random();
			m_random.InitState();
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var ecb = m_ecbSystem.CreateCommandBuffer().ToConcurrent();
			float r = m_random.NextFloat(0.0f, 1.0f);

			JobHandle jobHandle = Entities
				.WithAll<BrickTag>()
				.ForEach((Entity entity, int entityInQueryIndex, in Level level, in Translation translation) =>
				{
					if (level.m_level == 0)
					{
						ecb.DestroyEntity(entityInQueryIndex, entity);

						if (r < GameConfig.k_multiball10Value)
						{
							Entity powerupRequest = ecb.CreateEntity(entityInQueryIndex);
							PowerupData powerup = new PowerupData { m_powerup = PowerupType.None, m_context = 0.0f };

							if (r <= GameConfig.k_paddleSpeedUpValue)
							{
								powerup = new PowerupData { m_powerup = PowerupType.PaddleSpeed, m_context = GameConfig.k_paddleSpeedMultiplier };
							}
							else if (r <= GameConfig.k_ballSpeedUpValue)
							{
								powerup = new PowerupData { m_powerup = PowerupType.BallSpeed, m_context = GameConfig.k_ballSpeedMultiplier };
							}
							else if (r <= GameConfig.k_ballSpeedDownValue)
							{
								powerup = new PowerupData { m_powerup = PowerupType.BallSpeed, m_context = 1.0f / GameConfig.k_ballSpeedMultiplier };
							}
							else if (r <= GameConfig.k_multiball1Value)
							{
								powerup = new PowerupData { m_powerup = PowerupType.Multiball, m_context = 1.0f };
							}
							else if (r <= GameConfig.k_multiball2Value)
							{
								powerup = new PowerupData { m_powerup = PowerupType.Multiball, m_context = 2.0f };
							}
							else if (r <= GameConfig.k_multiball3Value)
							{
								powerup = new PowerupData { m_powerup = PowerupType.Multiball, m_context = 3.0f };
							}
							else if (r <= GameConfig.k_multiball5Value)
							{
								powerup = new PowerupData { m_powerup = PowerupType.Multiball, m_context = 5.0f };
							}
							else if (r <= GameConfig.k_multiball10Value)
							{
								powerup = new PowerupData { m_powerup = PowerupType.Multiball, m_context = 10.0f };
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
