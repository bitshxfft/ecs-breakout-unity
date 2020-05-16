using Breakout.Component.Collision;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Breakout.System.Collision
{
	[UpdateAfter(typeof(CollisionResetSystem))]
	public class BSphereVsBSphereCollisionSystem : CollisionSystem<BSphere>
	{
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var otherBSpheres = m_colliderQuery.ToComponentDataArray<BSphere>(Allocator.TempJob);
			var otherTranslations = m_colliderQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
			var otherEntities = m_colliderQuery.ToEntityArray(Allocator.TempJob);

			JobHandle jobHandle = Entities
				.WithDeallocateOnJobCompletion(otherBSpheres)
				.WithDeallocateOnJobCompletion(otherTranslations)
				.WithDeallocateOnJobCompletion(otherEntities)
				.WithBurst(FloatMode.Fast, FloatPrecision.Low)
				.ForEach((Entity entity, DynamicBuffer<CollisionEvent> collisionEvents, in BSphere bSphere, in Translation translation) =>
				{
					float2 position = new float2(translation.Value.x, translation.Value.y);
					float radius = bSphere.m_radius;
					float radiusSqr = radius * radius;

					for (int i = 0, count = otherEntities.Length; i < count; ++i)
					{
						// don't collide with ourselves
						if (entity.Index == otherEntities[i].Index)
						{
							continue;
						}

						// only collide with entities on layers within our filter
						if ((bSphere.m_collisionFilter & otherBSpheres[i].m_collisionLayer) == 0)
						{
							continue;
						}

						float2 otherPosition = new float2(otherTranslations[i].Value.x, otherTranslations[i].Value.y);
						float otherRadius = otherBSpheres[i].m_radius;
						float otherRadiusSqr = otherRadius * otherRadius;

						// check for intersection
						if (math.distancesq(position, otherPosition) > radiusSqr + otherRadiusSqr)
						{
							continue;
						}
							
						// add collision event
						collisionEvents.Add(
							new CollisionEvent
							{
								m_collisionData = new CollisionData
								{
									m_otherMin = otherPosition - new float2(otherRadius, otherRadius),
									m_otherMax = otherPosition + new float2(otherRadius, otherRadius),
									m_otherLayer = otherBSpheres[i].m_collisionFilter,
								}
							});
					}
				})
				.Schedule(inputDeps);

			return jobHandle;
		}
	}
}
