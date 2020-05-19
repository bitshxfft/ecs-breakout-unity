using Breakout.Component.Collision;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

// #SD-TODO >>> scale colliders

namespace Breakout.System.Collision
{
	[UpdateAfter(typeof(CollisionResetSystem))]
	public class AABBVsAABBCollisionSystem : CollisionSystem<AABB>
	{
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var otherAABBs = m_colliderQuery.ToComponentDataArray<AABB>(Allocator.TempJob);
			var otherTranslations = m_colliderQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
			var otherEntities = m_colliderQuery.ToEntityArray(Allocator.TempJob);

			JobHandle jobHandle = Entities
				.WithDeallocateOnJobCompletion(otherAABBs)
				.WithDeallocateOnJobCompletion(otherTranslations)
				.WithDeallocateOnJobCompletion(otherEntities)
				.WithBurst(FloatMode.Fast, FloatPrecision.Low)
				.ForEach((Entity entity, DynamicBuffer<CollisionEvent> collisionEvents, in AABB aabb, in Translation translation) =>
				{
					float2 position = new float2(translation.Value.x, translation.Value.y);
					float2 min = position + aabb.m_bottomLeft;
					float2 max = position + aabb.m_topRight;

					for (int i = 0, count = otherEntities.Length; i < count; ++i)
					{
						// don't collide with ourselves
						if (entity.Index == otherEntities[i].Index)
						{
							continue;
						}
							
						// only collide with entities on layers within our filter
						if ((aabb.m_collisionFilter & otherAABBs[i].m_collisionLayer) == 0)
						{
							continue;
						}

						float2 otherPosition = new float2(otherTranslations[i].Value.x, otherTranslations[i].Value.y);
						float2 otherMin = otherPosition + otherAABBs[i].m_bottomLeft;
						float2 otherMax = otherPosition + otherAABBs[i].m_topRight;

						// check for intersection
						if (max.x <= otherMin.x
							|| min.x >= otherMax.x
							|| max.y <= otherMin.y
							|| min.y >= otherMax.y)
						{
							continue;
						}

						//Debug.LogFormat("[AABBVsAABBColisionSystem] collision between (layer: {0}, min: {1}, max: {2}) and (layer: {3}, min: {4}, max: {5})\n",
						//	aabb.m_collisionLayer, min, max,
						//	otherAABBs[i].m_collisionLayer, otherMin, otherMax);

						collisionEvents.Add(
							new CollisionEvent 
							{
								m_collisionData = new CollisionData
								{
									m_otherMin = otherMin,
									m_otherMax = otherMax,
									m_otherLayer = otherAABBs[i].m_collisionLayer,
								}
							});
					}
				})
				.Schedule(inputDeps);

			return jobHandle;
		}
	}
}
