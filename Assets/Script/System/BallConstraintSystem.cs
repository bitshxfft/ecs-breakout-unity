using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(PixelPerfectMoveSystem))]
public class BallConstraintSystem : JobComponentSystem
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
		// #SteveD >>> this never changes, store somewhere?
		NativeArray<AABBData> playFieldBounds = m_playfieldQuery.ToComponentDataArray<AABBData>(Allocator.TempJob);
		AABBData playFieldAABB = playFieldBounds[0];
		playFieldBounds.Dispose();
		// <<<<<<<<<<<

		JobHandle jobHandle = Entities
			.WithAll<BallTag>()
			.ForEach((ref Translation translation, ref MoveData moveData, in AABBData aabb) =>
			{
				float minX = playFieldAABB.m_bottomLeft.x - aabb.m_bottomLeft.x;
				float maxX = playFieldAABB.m_topRight.x - aabb.m_topRight.x;
				float maxY = playFieldAABB.m_topRight.y - aabb.m_topRight.y;
				
				float3 position = translation.Value;
				float3 direction = moveData.m_direction;

				if (position.x < minX)
				{
					position.x = minX;
					direction.x = -direction.x;
				}
				else if (position.x > maxX)
				{
					position.x = maxX;
					direction.x = -direction.x;
				}

				if (position.y > maxY)
				{
					position.y = maxY;
					direction.y = -direction.y;
				}

				translation.Value = position;
				moveData.m_direction = direction;
			})
			.Schedule(inputDeps);

		return jobHandle;
	}
}
