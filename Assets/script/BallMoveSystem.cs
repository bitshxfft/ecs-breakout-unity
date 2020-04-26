using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class BallMoveSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		float dt = Time.DeltaTime;

		Entities.ForEach((ref Translation translation, in BallMoveData ballMoveData) =>
		{
			float2 step = (ballMoveData.m_direction * ballMoveData.m_speed * dt);
			translation.Value.x += step.x;
			translation.Value.y += step.y;

			// #SteveD >>> bounce off of top, left, right edges of screen
			//			>> drop below bottom of screen & respawn

			// #SteveD >>> increase ball speed every x bounces

		}).Run();

		return default;
	}
}
