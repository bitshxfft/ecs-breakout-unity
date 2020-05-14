using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

// #SD >>> refactor playfield bounds into standalone MonoBehaviour manager (GameManager.cs)

public class BallUnspawnSystem : JobComponentSystem
{
	private AABB m_playfieldBounds = default;
	private EndSimulationEntityCommandBufferSystem m_ecbSystem = default;

	// --------------------------------------------------------------------------------

	protected override void OnCreate()
	{
		base.OnCreate();

		m_ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
	}

	protected override void OnStartRunning()
	{
		base.OnStartRunning();

		EntityQuery playfieldQuery = GetEntityQuery(ComponentType.ReadOnly<PlayFieldTag>(), ComponentType.ReadOnly<AABB>());
		NativeArray<AABB> playFieldBounds = playfieldQuery.ToComponentDataArray<AABB>(Allocator.Temp);
		m_playfieldBounds = playFieldBounds[0];
		playFieldBounds.Dispose();
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		AABB playfieldBounds = m_playfieldBounds;

		EntityCommandBuffer.Concurrent ecb = m_ecbSystem.CreateCommandBuffer().ToConcurrent();
		JobHandle jobHandle = Entities
			.WithAll<BallTag>()
			.ForEach((Entity ballEntity, int entityInQueryIndex, in Translation translation, in AABB aabb) =>
			{
				float minY = playfieldBounds.m_bottomLeft.y - aabb.m_topRight.y;
				
				if (translation.Value.y < minY)
				{
					ecb.DestroyEntity(entityInQueryIndex, ballEntity);
				}
			})
			.Schedule(inputDeps);
		
		return jobHandle;
	}
}
