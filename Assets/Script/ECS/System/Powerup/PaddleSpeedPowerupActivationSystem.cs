using Breakout.Component.Movement;
using Breakout.Component.Powerup;
using Breakout.Component.Tag;
using Breakout.System.Spawn;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Breakout.System.Powerup
{
	[UpdateAfter(typeof(PowerupSpawnSystem))]
	public sealed class PaddleSpeedPowerupActivationSystem : PowerupActivationSystem<PaddleSpeedPowerupActivationRequest>
	{
		private EntityQuery m_paddleQuery = default;

		// --------------------------------------------------------------------------------

		protected override void OnCreate()
		{
			base.OnCreate();

			m_paddleQuery = GetEntityQuery(
				ComponentType.ReadWrite<PaddleTag>(), 
				ComponentType.ReadWrite<Speed>());
		}
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			EntityCommandBuffer.Concurrent ecb = m_ecbSystem.CreateCommandBuffer().ToConcurrent();
			var paddleSpeeds = m_paddleQuery.ToComponentDataArray<Speed>(Allocator.TempJob);

			JobHandle jobHandle = Entities
				.WithDeallocateOnJobCompletion(paddleSpeeds)
				.ForEach((Entity entity, int entityInQueryIndex, in PaddleSpeedPowerupActivationRequest request) =>
				{
					for (int i = 0; i < paddleSpeeds.Length; ++i)
					{
						Speed speed = paddleSpeeds[i];
						speed.m_speed *= request.m_speedlMultiplier;
						paddleSpeeds[i] = speed;
					}

					ecb.DestroyEntity(entityInQueryIndex, entity);
				})
				.Schedule(inputDeps);

			m_ecbSystem.AddJobHandleForProducer(jobHandle);
			return jobHandle;
		}
	}
}
