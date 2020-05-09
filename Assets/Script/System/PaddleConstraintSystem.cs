using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(PixelPerfectMoveSystem))]
public class PaddleConstraintSystem : JobComponentSystem
{
	private EntityQuery m_playfieldQuery = default;
	
	// --------------------------------------------------------------------------------

	protected override void OnCreate()
	{
		base.OnCreate();

		m_playfieldQuery = GetEntityQuery(
			ComponentType.ReadOnly<PlayFieldTag>(),
			ComponentType.ReadOnly<AABBData>());
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		// #SD-TODO >>> this never changes, store somewhere?
		NativeArray<AABBData> playFieldBounds = m_playfieldQuery.ToComponentDataArray<AABBData>(Allocator.TempJob);
		AABBData playFieldAABB = playFieldBounds[0];
		playFieldBounds.Dispose();
		// <<<<<<<<<<<

		JobHandle jobHandle = Entities
			.WithAll<PaddleTag>()
			.ForEach((ref Translation translation, in AABBData aabb) =>
			{
				translation.Value.x = math.clamp(
					translation.Value.x, 
					playFieldAABB.m_bottomLeft.x - aabb.m_bottomLeft.x, 
					playFieldAABB.m_topRight.x - aabb.m_topRight.x);
			})
			.Schedule(inputDeps);

		return jobHandle;
	}
}
