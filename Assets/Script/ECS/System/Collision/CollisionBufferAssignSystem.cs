using Breakout.Component.Collision;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Breakout.System.Collision
{
	public class CollisionBufferAssignSystem : JobComponentSystem
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
				.WithAny<AABB, BSphere>()
				.WithNone<CollisionEvent>()
				.ForEach((Entity entity, int entityInQueryIndex) =>
				{
					//Debug.Log("[CollisionBufferAssignSystem] Assigned collision buffer\n");
					ecb.AddBuffer<CollisionEvent>(entityInQueryIndex, entity);
				})
				.Schedule(inputDeps);

			m_ecbSystem.AddJobHandleForProducer(jobHandle);
			return jobHandle;
		}
	}
}
