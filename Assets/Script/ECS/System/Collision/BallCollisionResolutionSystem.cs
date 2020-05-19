using Breakout.Component.Collision;
using Breakout.Component.Movement;
using Breakout.Component.Tag;
using Breakout.Config;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Breakout.System.Collision
{
	[UpdateAfter(typeof(BSphereVsAABBCollisionSystem))]
	public class BallCollisionResolutionSystem : JobComponentSystem
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
				.WithAll<BallTag>()
				.ForEach((Entity entity, int entityInQueryIndex, DynamicBuffer<CollisionEvent> collisionEvents, ref Translation translation, ref Direction direction, in BSphere bSphere, in Displacement displacement) =>
				{
					for (int i = 0, count = collisionEvents.Length; i < count; ++i)
					{
						CollisionData collisionData = collisionEvents[i].m_collisionData;
						//Debug.LogFormat("[BallCollisionResolutionSystem] Ball collided with layer: {0}\n", collisionData.m_otherLayer);

						if ((collisionData.m_otherLayer & CollisionLayer.Killzone) > 0)
						{
							ecb.DestroyEntity(entityInQueryIndex, entity);
						}
						else if ((collisionData.m_otherLayer & CollisionLayer.LevelBounds) > 0
							|| (collisionData.m_otherLayer & CollisionLayer.Brick) > 0)
						{
							float2 position = new float2(translation.Value.x, translation.Value.y);
							float2 min = position - new float2(bSphere.m_radius, bSphere.m_radius);
							float2 max = position + new float2(bSphere.m_radius, bSphere.m_radius);
							float2 prevPosition = position - displacement.m_displacement;
							float2 prevMin = min - displacement.m_displacement;
							float2 prevMax = max - displacement.m_displacement;

							float2 otherMin = collisionEvents[i].m_collisionData.m_otherMin;
							float2 otherMax = collisionEvents[i].m_collisionData.m_otherMax;

							if (min.x < otherMax.x && prevMin.x >= otherMax.x)
							{
								translation.Value.x = otherMax.x + bSphere.m_radius;
								direction.m_direction = new float2(-direction.m_direction.x, direction.m_direction.y);
							}
							else if (max.x > otherMin.x && prevMax.x <= otherMin.x)
							{
								translation.Value.x = otherMin.x - bSphere.m_radius;
								direction.m_direction = new float2(-direction.m_direction.x, direction.m_direction.y);
							}

							if (min.y < otherMax.y && prevMin.y >= otherMax.y)
							{
								translation.Value.y = otherMax.y + bSphere.m_radius;
								direction.m_direction = new float2(direction.m_direction.x, -direction.m_direction.y);
							}
							else if (max.y > otherMin.y && prevMax.y <= otherMin.y)
							{
								translation.Value.y = otherMin.y - bSphere.m_radius;
								direction.m_direction = new float2(direction.m_direction.x, -direction.m_direction.y);
							}
						}
						else if ((collisionData.m_otherLayer & CollisionLayer.Paddle) > 0)
						{
							float2 otherMin = collisionEvents[i].m_collisionData.m_otherMin;
							float2 otherMax = collisionEvents[i].m_collisionData.m_otherMax;
							float2 otherSize = (otherMax - otherMin);
							float2 otherCentre = otherMin + (otherSize * 0.5f);
							float hitDelta = (translation.Value.x - otherCentre.x) / (otherSize.x * 0.5f);

							float angle = math.radians(GameConfig.k_maxPaddleBallReflectAngle * -hitDelta);
							direction.m_direction = new float2(-math.sin(angle), math.cos(angle));
						}
					}
				})
				.Schedule(inputDeps);

			m_ecbSystem.AddJobHandleForProducer(jobHandle);
			return jobHandle;
		}
	}
}
