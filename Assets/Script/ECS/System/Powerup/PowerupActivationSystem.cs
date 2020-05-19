using Breakout.Component.Powerup;
using Breakout.Component.Prefab;
using Breakout.System.Collision;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Breakout.System.Spawn
{
	[UpdateAfter(typeof(PowerupSpawnSystem))]
	public class PowerupActivationSystem : JobComponentSystem
	{
		private EndSimulationEntityCommandBufferSystem m_ecbSystem = default;
		
		// --------------------------------------------------------------------------------

		protected override void OnCreate()
		{
			base.OnCreate();

			m_ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			EntityCommandBuffer.Concurrent ecb = m_ecbSystem.CreateCommandBuffer().ToConcurrent();
			
			// #SD-TODO >>> split into a system and a component spawned per powerup type (BallSpeed, Multiball, PaddleSize, PaddleSpeed)

			JobHandle jobHandle = Entities
				.ForEach((Entity entity, int entityInQueryIndex, in PowerupActivationRequest request) =>
				{
					//switch (request.m_powerup.m_powerup)
					//{
					//	case PowerupType.BallSpeed:
					//		// #SD-TODO >>> for all balls
					//		break;
					//	case PowerupType.Multiball:
					//		// #SD-TODO >>> create more balls from paddle
					//		break;
					//	case PowerupType.PaddleSize:
					//		// #SD-TODO >>> for all paddles
					//		break;
					//	case PowerupType.PaddleSpeed:
					//		// #SD-TODO >>> for all paddles
					//		break;
					//}

					ecb.DestroyEntity(entityInQueryIndex, entity);
				})
				.Schedule(inputDeps);

			m_ecbSystem.AddJobHandleForProducer(jobHandle);
			return jobHandle;
		}
	}
}
