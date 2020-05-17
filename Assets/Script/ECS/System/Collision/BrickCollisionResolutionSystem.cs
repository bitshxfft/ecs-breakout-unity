using Breakout.Component.Brick;
using Breakout.Component.Collision;
using Breakout.Component.Tag;
using Unity.Entities;
using Unity.Jobs;

namespace Breakout.System.Collision
{
	[UpdateAfter(typeof(AABBVsBSphereCollisionSystem))]
	public class BrickCollisionResolutionSystem : JobComponentSystem
	{
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			JobHandle jobHandle = Entities
				.WithAll<BrickTag>()
				.ForEach((DynamicBuffer<CollisionEvent> collisionEvents, ref Level level) =>
				{
					if (collisionEvents.Length > 0)
					{
						level.m_level -= 1;
					}
				})
				.Schedule(inputDeps);

			return jobHandle;
		}
	}
}
