using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;

[AlwaysSynchronizeSystem]
public class PlayerInputSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		Entities
			.ForEach((ref MoveData moveData, in PlayerInputData playerInputData) =>
			{
				int direction = 0;
				direction += Input.GetKey(playerInputData.m_leftKey) ? -1 : 0;
				direction += Input.GetKey(playerInputData.m_rightKey) ? 1 : 0;
				moveData.m_direction = new float3(direction, 0.0f, 0.0f);
			})
		.Run();

		return default;
	}
}
