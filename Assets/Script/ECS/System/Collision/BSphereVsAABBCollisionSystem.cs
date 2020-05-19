using Breakout.Component.Collision;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// #SD-TODO >>> scale colliders

namespace Breakout.System.Collision
{
	[UpdateAfter(typeof(CollisionResetSystem))]
	public class BSphereVsAABBCollisionSystem : CollisionSystem<AABB>
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
			var otherAABBs = m_colliderQuery.ToComponentDataArray<AABB>(Allocator.TempJob);
			var otherTranslations = m_colliderQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
			var otherEntities = m_colliderQuery.ToEntityArray(Allocator.TempJob);

			JobHandle jobHandle = Entities
				.WithDeallocateOnJobCompletion(otherAABBs)
				.WithDeallocateOnJobCompletion(otherTranslations)
				.WithDeallocateOnJobCompletion(otherEntities)
				.WithBurst(FloatMode.Fast, FloatPrecision.Low)
				.ForEach((Entity entity, DynamicBuffer<CollisionEvent> collisionEvents, in BSphere bSphere, in Translation translation) =>
				{
					float2 position = new float2(translation.Value.x, translation.Value.y);
					float radius = bSphere.m_radius;

					for (int i = 0, count = otherEntities.Length; i < count; ++i)
					{
						if (entity.Index == otherEntities[i].Index
							|| (bSphere.m_collisionFilter & otherAABBs[i].m_collisionLayer) == 0)
						{
							continue;
						}

						float2 otherPosition = new float2(otherTranslations[i].Value.x, otherTranslations[i].Value.y);
						float2 otherMin = otherPosition + otherAABBs[i].m_bottomLeft;
						float2 otherMax = otherPosition + otherAABBs[i].m_topRight;

						if (otherMin.x > position.x + radius
							|| otherMax.x < position.x - radius
							|| otherMin.y > position.y + radius
							|| otherMax.y < position.y - radius)
						{
							continue;
						}

						if ((bSphere.m_collisionFilter & otherAABBs[i].m_collisionLayer) > 0)
						{
							//Debug.LogFormat("[BSphereVsAABBCollisionSystem] collision between (layer: {0}, position: {1}, radius: {2}) and (layer: {3}, min: {4}, max: {5})\n",
							//	bSphere.m_collisionLayer, position, radius,
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
					}
				})
				.Schedule(inputDeps);

			m_ecbSystem.AddJobHandleForProducer(jobHandle);
			return jobHandle;
		}
	}
}
