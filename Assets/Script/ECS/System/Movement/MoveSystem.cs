using Breakout.Component.Movement;
using Breakout.System.Collision;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Breakout.System.Movement
{
	[UpdateAfter(typeof(CollisionResetSystem))]
	public class MoveSystem : JobComponentSystem
	{
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			float dt = Time.DeltaTime;

			JobHandle jobHandle = Entities
				.WithNone<BlockMovement>()
				.ForEach((ref Translation translation, ref Displacement displacement, in Direction direction, in Speed speed) =>
				{
					float2 step = direction.m_direction * speed.m_speed * dt;
					displacement.m_displacement = step;
					translation.Value.x += step.x;
					translation.Value.y += step.y;
				})
				.Schedule(inputDeps);

			return jobHandle;
		}
	}
}
