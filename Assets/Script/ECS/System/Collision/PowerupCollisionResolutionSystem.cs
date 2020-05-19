using Breakout.Component.Collision;
using Breakout.Component.Powerup;
using Breakout.Component.Tag;
using Unity.Entities;
using Unity.Jobs;

namespace Breakout.System.Collision
{
	[UpdateAfter(typeof(AABBVsAABBCollisionSystem))]
	public class PowerupCollisionResolutionSystem : JobComponentSystem
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
				.WithAll<PowerupTag>()
				.ForEach((Entity entity, int entityInQueryIndex, DynamicBuffer<CollisionEvent> collisionEvents, in PowerupData powerup) =>
				{
					for (int i = 0, count = collisionEvents.Length; i < count; ++i)
					{
						CollisionData collisionData = collisionEvents[i].m_collisionData;
						
						if ((collisionData.m_otherLayer & CollisionLayer.Killzone) > 0)
						{
							ecb.DestroyEntity(entityInQueryIndex, entity);
						}
						else if ((collisionData.m_otherLayer & CollisionLayer.Paddle) > 0)
						{
							ecb.DestroyEntity(entityInQueryIndex, entity);

							Entity actionRequest = ecb.CreateEntity(entityInQueryIndex);
							switch (powerup.m_powerup)
							{
								case PowerupType.PaddleSpeed:
									ecb.AddComponent(entityInQueryIndex, actionRequest, 
										new PaddleSpeedPowerupActivationRequest 
										{
											m_speedlMultiplier = powerup.m_context,
										});
									break;

								case PowerupType.BallSpeed:
									ecb.AddComponent(entityInQueryIndex, actionRequest, 
										new BallSpeedPowerupActivationRequest 
										{
											m_speedlMultiplier = powerup.m_context,
										});

									break;
								case PowerupType.Multiball:
									ecb.AddComponent(entityInQueryIndex, actionRequest, 
										new MultiballPowerupActivationRequest 
										{
											m_ballCount = (int)powerup.m_context,
										});
									break;
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
