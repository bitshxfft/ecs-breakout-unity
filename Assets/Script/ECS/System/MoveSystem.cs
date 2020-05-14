using Breakout.Component;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Breakout.System
{
	public class MoveSystem : JobComponentSystem
	{
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			float dt = Time.DeltaTime;

			JobHandle jobHandle = Entities
				.WithNone<BlockMovement>()
				.ForEach((ref Translation translation, in Direction direction, in Speed speed) =>
				{
					float2 step = direction.m_direction * speed.m_speed * dt;
					translation.Value.x += step.x;
					translation.Value.y += step.y;
				})
				.Schedule(inputDeps);

			return jobHandle;
		}
	}
}
