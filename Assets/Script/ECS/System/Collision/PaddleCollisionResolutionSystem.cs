using Breakout.Component.Collision;
using Breakout.Component.Movement;
using Breakout.Component.Tag;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Breakout.System.Collision
{
	[UpdateAfter(typeof(AABBVsAABBCollisionSystem))]
	public class PaddleCollisionResolutionSystem : JobComponentSystem
	{
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			JobHandle jobHandle = Entities
				.WithAll<PaddleTag>()
				.ForEach((Entity entity, int entityInQueryIndex, DynamicBuffer<CollisionEvent> collisionEvents, ref Translation translation, in AABB aabb, in Displacement displacement) =>
				{
					int eventCount = collisionEvents.Length;
					if (eventCount > 0)
					{
						float2 position = new float2(translation.Value.x, translation.Value.y);
						float2 min = position + aabb.m_bottomLeft;
						float2 max = position + aabb.m_topRight;
						float2 prevPosition = position - displacement.m_displacement;
						float2 prevMin = min - displacement.m_displacement;
						float2 prevMax = max - displacement.m_displacement;

						for (int i = 0; i < eventCount; ++i)
						{
							if ((collisionEvents[i].m_collisionData.m_otherLayer & CollisionLayer.LevelBounds) > 0)
							{
								float2 otherMin = collisionEvents[i].m_collisionData.m_otherMin;
								float2 otherMax = collisionEvents[i].m_collisionData.m_otherMax;

								if (min.x < otherMax.x && prevMin.x >= otherMax.x)
								{
									translation.Value.x = otherMax.x - aabb.m_bottomLeft.x;
								}
								else if (max.x > otherMin.x && prevMax.x <= otherMin.x)
								{
									translation.Value.x = otherMin.x - aabb.m_topRight.x;
								}
							}
						}
					}
				})
				.Schedule(inputDeps);

			return jobHandle;
		}
	}
}
