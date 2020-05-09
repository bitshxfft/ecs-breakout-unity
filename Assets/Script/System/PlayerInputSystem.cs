using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;

[AlwaysSynchronizeSystem]
public class PlayerInputSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		// move paddle(s)
		Entities
			.ForEach((ref MoveData moveData, in PaddleInputData paddleInputData) =>
			{
				int direction = 0;
				direction += Input.GetKey(paddleInputData.m_leftKey) ? -1 : 0;
				direction += Input.GetKey(paddleInputData.m_rightKey) ? 1 : 0;
				moveData.m_direction = new float3(direction, 0.0f, 0.0f);
			})
			.Run();

		// launch ball(s)
		EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
		Entities
			.WithAll<BallTag>()
			.WithNone<PixelPerfectTranslationData>()
			.ForEach((Entity ballEntity, ref Translation translation, in BallInputData ballInputData, in LocalToWorld localToWorld) =>
			{
				if (Input.GetKeyDown(ballInputData.m_launchKey))
				{
					ecb.SetComponent<Translation>(ballEntity, new Translation() { Value = localToWorld.Position });
					ecb.RemoveComponent<Parent>(ballEntity);
					ecb.RemoveComponent<LocalToParent>(ballEntity);
					ecb.AddComponent<PixelPerfectTranslationData>(ballEntity);
				}
			})
			.Run();
		ecb.Playback(EntityManager);
		ecb.Dispose();

		return inputDeps;
	}
}
