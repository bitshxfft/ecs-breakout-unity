using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class BallConstraintSystem : JobComponentSystem
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
			.WithAll<BallTag>()
			.ForEach((ref Translation translation, ref Direction direction, in AABB aabb) =>
			{
				float minX = playfieldBounds.m_bottomLeft.x - aabb.m_bottomLeft.x;
				float maxX = playfieldBounds.m_topRight.x - aabb.m_topRight.x;
				float maxY = playfieldBounds.m_topRight.y - aabb.m_topRight.y;
				
				if (translation.Value.x < minX)
				{
					translation.Value.x = minX;
					direction.m_direction.x = -direction.m_direction.x;
				}
				else if (translation.Value.x > maxX)
				{
					translation.Value.x = maxX;
					direction.m_direction.x = -direction.m_direction.x;
				}

				if (translation.Value.y > maxY)
				{
					translation.Value.y = maxY;
					direction.m_direction.y = -direction.m_direction.y;
				}
			})
			.Schedule(inputDeps);

		return jobHandle;
	}
}
