using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;

[AlwaysSynchronizeSystem]
public class BallInputSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
		Entities
			.WithAll<BallTag>()
			.WithNone<Parent>()
			.ForEach((Entity ballEntity, ref Translation translation, in BallInput ballInputData, in LocalToWorld localToWorld) =>
			{
				if (Input.GetKeyDown(ballInputData.m_launchKey))
				{
					ecb.SetComponent(ballEntity, new Translation() { Value = localToWorld.Position });
					ecb.RemoveComponent<Parent>(ballEntity);
					ecb.RemoveComponent<LocalToParent>(ballEntity);
				}
			})
			.Run();
		ecb.Playback(EntityManager);
		ecb.Dispose();

		return inputDeps;
	}
}
