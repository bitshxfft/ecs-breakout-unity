using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class PaddleMoveSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		float dt = Time.DeltaTime;

		Entities.ForEach((ref Translation translation, in PaddleMoveData paddleMoveData) =>
		{
			float step = (paddleMoveData.m_direction * paddleMoveData.m_speed * dt);
			translation.Value.x = translation.Value.x + step;

			// #SteveD >>> clamp within screen bounds

		}).Run();
		
		return default;
	}
}
