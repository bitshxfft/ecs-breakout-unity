using Breakout.Component.Collision;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Breakout.System.Collision
{
	[UpdateAfter(typeof(CollisionBufferAssignSystem))]
	public class CollisionResetSystem : JobComponentSystem
	{
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			JobHandle jobHandle = Entities
				.ForEach((DynamicBuffer<CollisionEvent> collisionEvents) =>
				{
					collisionEvents.Clear();
				})
				.Schedule(inputDeps);

			return jobHandle;
		}
	}
}
