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
				.ForEach((Entity entity, int entityInQueryIndex, DynamicBuffer<CollisionEvent> collisionEvents, in Powerup powerup) =>
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
							ecb.AddComponent(entityInQueryIndex, actionRequest, new PowerupActivationRequest { m_powerup = powerup });
						}
					}
				})
				.Schedule(inputDeps);

			m_ecbSystem.AddJobHandleForProducer(jobHandle);
			return jobHandle;
		}
	}
}
