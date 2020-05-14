using Breakout.Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Breakout.System
{
	[UpdateAfter(typeof(MoveSystem))]
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
				ComponentType.ReadOnly<AABB>());
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			NativeArray<Translation> paddleTranslations = m_paddleQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
			NativeArray<AABB> paddleAABBs = m_paddleQuery.ToComponentDataArray<AABB>(Allocator.TempJob);

			JobHandle jobHandle = Entities
				.WithAll<BallTag>()
				.WithNone<BlockMovement>()
				.WithDeallocateOnJobCompletion(paddleTranslations)
				.WithDeallocateOnJobCompletion(paddleAABBs)
				.ForEach((ref Translation translation, ref Direction direction, in AABB aabb) =>
				{
					float2 ballMin = new float2(translation.Value.x, translation.Value.y);
					ballMin.x += aabb.m_bottomLeft.x;
					ballMin.y += aabb.m_bottomLeft.y;

					float2 ballMax = new float2(translation.Value.x, translation.Value.y);
					ballMax.x += aabb.m_topRight.x;
					ballMax.y += aabb.m_topRight.y;

					for (int i = 0, count = paddleTranslations.Length; i < count; ++i)
					{
						float3 paddlePosition = paddleTranslations[i].Value;
						AABB paddleAABB = paddleAABBs[i];

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
							translation.Value.y = paddlePosition.y + paddleAABB.m_topRight.y - aabb.m_bottomLeft.y;
						
							float bounceDelta = (translation.Value.x - paddlePosition.x) / ((paddleMax.x - paddleMin.x) * 0.5f);
							
							// #SD >>> get angle limits from component on paddle
							float angle = math.radians(45.0f * -bounceDelta); 
							// <<<<<<<

							direction.m_direction = new float2(-math.sin(angle), math.cos(angle));
						}
					}
				})
				.Schedule(inputDeps);

			return jobHandle;
		}
	}
}
