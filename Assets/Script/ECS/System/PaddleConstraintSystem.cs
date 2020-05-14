using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(MoveSystem))]
public class PaddleConstraintSystem : JobComponentSystem
{
	private AABB m_playfieldBounds = default;

	// --------------------------------------------------------------------------------

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

		JobHandle jobHandle = Entities
			.WithAll<PaddleTag>()
			.ForEach((ref Translation translation, in AABB aabb) =>
			{
				translation.Value.x = math.clamp(
					translation.Value.x, 
					playfieldBounds.m_bottomLeft.x - aabb.m_bottomLeft.x,
					playfieldBounds.m_topRight.x - aabb.m_topRight.x);
			})
			.Schedule(inputDeps);

		return jobHandle;
	}
}
