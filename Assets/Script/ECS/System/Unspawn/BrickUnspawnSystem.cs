using Breakout.Component.Brick;
using Breakout.Component.Tag;
using Breakout.System.Collision;
using Unity.Entities;
using Unity.Jobs;

namespace Breakout.System.Unspawn
{
	[UpdateAfter(typeof(BrickCollisionResolutionSystem))]
	public class BrickUnspawnSystem : JobComponentSystem
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
				.WithAll<BrickTag>()
				.ForEach((Entity entity, int entityInQueryIndex, in Level level) =>
				{
					if (level.m_level == 0)
					{
						ecb.DestroyEntity(entityInQueryIndex, entity);
					}
				})
				.Schedule(inputDeps);

			m_ecbSystem.AddJobHandleForProducer(jobHandle);
			return jobHandle;
		}
	}
}
