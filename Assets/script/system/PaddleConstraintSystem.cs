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
		m_playfieldQuery = GetEntityQuery(
			ComponentType.ReadOnly<PlayFieldTag>(),
			ComponentType.ReadOnly<AABBData>());
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		NativeArray<AABBData> playFieldBounds = m_playfieldQuery.ToComponentDataArray<AABBData>(Allocator.TempJob);
		AABBData playFieldAABB = playFieldBounds[0];
		playFieldBounds.Dispose();

		Entities
			.ForEach((ref Translation translation, in AABBData aabb, in PaddleTag tag) =>
			{
				translation.Value.x = math.clamp(
					translation.Value.x, 
					playFieldAABB.m_bottomLeft.x - aabb.m_bottomLeft.x, 
					playFieldAABB.m_topRight.x - aabb.m_topRight.x);
			})
			.Run();

		return default;
	}
}
