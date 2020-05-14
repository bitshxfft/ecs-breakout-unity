using Breakout.Component;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;

namespace Breakout.System
{
	[AlwaysSynchronizeSystem]
	public class PaddleInputSystem : JobComponentSystem
	{
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			Entities
				.ForEach((ref Direction direction, in PaddleInput paddleInputData) =>
				{
					float x = 0;
					x += Input.GetKey(paddleInputData.m_leftKey) ? -1.0f : 0.0f;
					x += Input.GetKey(paddleInputData.m_rightKey) ? 1.0f : 0.0f;
					direction.m_direction = new float2(x, 0.0f);
				})
				.Run();

			return inputDeps;
		}
	}
}
