using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(PixelPerfectMoveSystem))]
public class PaddleBallCollisionSystem : JobComponentSystem
{
	private EntityQuery m_paddleQuery = default;

	// --------------------------------------------------------------------------------

	protected override void OnCreate()
	{
		base.OnCreate();

		m_paddleQuery = GetEntityQuery(
			ComponentType.ReadOnly<PaddleTag>(),
			ComponentType.ReadOnly<Translation>(),
			ComponentType.ReadOnly<AABBData>());
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		int paddleCount = m_paddleQuery.CalculateEntityCount();
		NativeArray<Translation> paddleTranslations = m_paddleQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
		NativeArray<AABBData> paddleAABBs = m_paddleQuery.ToComponentDataArray<AABBData>(Allocator.TempJob);

		JobHandle jobHandle = Entities
			.WithAll<BallTag>()
			.WithNone<Parent>()
			.WithDeallocateOnJobCompletion(paddleTranslations)
			.WithDeallocateOnJobCompletion(paddleAABBs)
			.ForEach((ref Translation translation, ref MoveData moveData, in AABBData aabb) =>
			{
				float3 ballPosition = translation.Value;
				float3 ballDirection = moveData.m_direction;

				float3 ballMin = ballPosition;
				ballMin.x += aabb.m_bottomLeft.x;
				ballMin.y += aabb.m_bottomLeft.y;

				float3 ballMax = ballPosition;
				ballMax.x += aabb.m_topRight.x;
				ballMax.y += aabb.m_topRight.y;

				for (int i = 0; i < paddleCount; ++i)
				{
					float3 paddlePosition = paddleTranslations[i].Value;
					AABBData paddleAABB = paddleAABBs[i];

					float3 paddleMin = paddlePosition;
					paddleMin.x += paddleAABB.m_bottomLeft.x;
					paddleMin.y += paddleAABB.m_bottomLeft.y;

					float3 paddleMax = paddlePosition;
					paddleMax.x += paddleAABB.m_topRight.x;
					paddleMax.y += paddleAABB.m_topRight.y;

					if (ballMax.x > paddleMin.x
						&& ballMin.x < paddleMax.x
						&& ballMax.y > paddleMin.y
						&& ballMin.y < paddleMax.y)
					{
						ballPosition.y = paddlePosition.y + paddleAABB.m_topRight.y - aabb.m_bottomLeft.y;
					
						float bounceDelta = (ballPosition.x - paddlePosition.x) / ((paddleMax.x - paddleMin.x) * 0.5f);
						float angle = math.radians(45.0f * -bounceDelta); // #SteveD >>> remove magic number -> put into component
						ballDirection = new float3(-math.sin(angle), math.cos(angle), 0.0f);
					}
				}

				translation.Value = ballPosition;
				moveData.m_direction = ballDirection;
			})
			.Schedule(inputDeps);

		return jobHandle;
	}
}
