using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class BallUnspawnSystem : JobComponentSystem
{
	private EntityQuery m_playfieldQuery = default;
	EndSimulationEntityCommandBufferSystem m_ecbSystem = default;

	// --------------------------------------------------------------------------------

	protected override void OnCreate()
	{
		base.OnCreate();

		m_playfieldQuery = GetEntityQuery(
			ComponentType.ReadOnly<PlayFieldTag>(),
			ComponentType.ReadOnly<AABBData>());

		m_ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		// #SteveD >>> this never changes, store somewhere?
		NativeArray<AABBData> playFieldBounds = m_playfieldQuery.ToComponentDataArray<AABBData>(Allocator.TempJob);
		AABBData playFieldAABB = playFieldBounds[0];
		playFieldBounds.Dispose();
		// <<<<<<<<<<<

		EntityCommandBuffer.Concurrent ecb = m_ecbSystem.CreateCommandBuffer().ToConcurrent();
		JobHandle jobHandle = Entities
			.WithAll<BallTag>()
			.ForEach((Entity ballEntity, int entityInQueryIndex, in Translation translation, in AABBData aabb) =>
			{
				float minY = playFieldAABB.m_bottomLeft.y - aabb.m_topRight.y;
				
				if (translation.Value.y < minY)
				{
					ecb.DestroyEntity(entityInQueryIndex, ballEntity);
				}
			})
			.Schedule(inputDeps);
		
		return jobHandle;
	}
}
