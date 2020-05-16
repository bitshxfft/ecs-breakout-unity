using Breakout.Component.Collision;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Breakout.System.Collision
{
	[UpdateAfter(typeof(CollisionResetSystem))]
	public class AABBVsBSphereCollisionSystem : CollisionSystem<BSphere>
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
			var otherBSpheres = m_colliderQuery.ToComponentDataArray<BSphere>(Allocator.TempJob);
			var otherTranslations = m_colliderQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
			var otherEntities = m_colliderQuery.ToEntityArray(Allocator.TempJob);

			JobHandle jobHandle = Entities
				.WithDeallocateOnJobCompletion(otherBSpheres)
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
						if (entity.Index == otherEntities[i].Index
							|| (aabb.m_collisionFilter & otherBSpheres[i].m_collisionLayer) == 0)
						{
							continue;
						}

						float2 otherPosition = new float2(otherTranslations[i].Value.x, otherTranslations[i].Value.y);
						float otherRadius = otherBSpheres[i].m_radius;

						if (min.x > otherPosition.x + otherRadius
							|| max.x < otherPosition.x - otherRadius
							|| min.y > otherPosition.y + otherRadius
							|| max.y < otherPosition.y - otherRadius)
						{
							continue;
						}

						//Debug.LogFormat("[AABBVsBSphereCollisionSystem] collision between (layer: {0}, min: {1}, max: {2}) and (layer: {3}, position: {4}, radius: {5})\n",
						//	aabb.m_collisionLayer, min, max,
						//	otherBSpheres[i].m_collisionLayer, otherPosition, otherRadius);

						collisionEvents.Add(
							new CollisionEvent
							{
								m_collisionData = new CollisionData
								{
									m_otherMin = otherPosition - new float2(otherRadius, otherRadius),
									m_otherMax = otherPosition + new float2(otherRadius, otherRadius),
									m_otherLayer = otherBSpheres[i].m_collisionLayer,
								}
							});
					}
				})
				.Schedule(inputDeps);

			m_ecbSystem.AddJobHandleForProducer(jobHandle);
			return jobHandle;
		}
	}
}
