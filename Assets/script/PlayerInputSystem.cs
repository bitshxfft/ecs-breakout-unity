using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class PlayerInputSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		Entities.ForEach((ref PaddleMoveData paddleMoveData, in PlayerInputData playerInputData) =>
		{
			int direction = 0;
			direction += Input.GetKey(playerInputData.m_leftKey) ? -1 : 0;
			direction += Input.GetKey(playerInputData.m_rightKey) ? 1 : 0;
			paddleMoveData.m_direction = direction;
		}).Run();

		return default;
	}
}
