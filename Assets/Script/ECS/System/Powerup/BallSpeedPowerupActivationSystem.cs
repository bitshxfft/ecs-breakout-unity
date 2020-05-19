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
	public sealed class BallSpeedPowerupActivationSystem : PowerupActivationSystem<BallSpeedPowerupActivationRequest>
	{
		private EntityQuery m_ballQuery = default;

		// --------------------------------------------------------------------------------

		protected override void OnCreate()
		{
			base.OnCreate();

			m_ballQuery = GetEntityQuery(
				ComponentType.ReadWrite<BallTag>(),
				ComponentType.ReadWrite<Speed>());
		}
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			EntityCommandBuffer.Concurrent ecb = m_ecbSystem.CreateCommandBuffer().ToConcurrent();
			var ballSpeeds = m_ballQuery.ToComponentDataArray<Speed>(Allocator.TempJob);

			JobHandle jobHandle = Entities
				.WithDeallocateOnJobCompletion(ballSpeeds)
				.ForEach((Entity entity, int entityInQueryIndex, in BallSpeedPowerupActivationRequest request) =>
				{
					for (int i = 0; i < ballSpeeds.Length; ++i)
					{
						Speed speed = ballSpeeds[i];
						speed.m_speed *= request.m_speedlMultiplier;
						ballSpeeds[i] = speed;
					}

					ecb.DestroyEntity(entityInQueryIndex, entity);
				})
				.Schedule(inputDeps);

			m_ecbSystem.AddJobHandleForProducer(jobHandle);
			return jobHandle;
		}
	}
}
