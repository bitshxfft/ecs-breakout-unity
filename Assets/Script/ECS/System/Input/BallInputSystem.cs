using Breakout.Component.Input;
using Breakout.Component.Tag;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Transforms;
using Breakout.Component.Movement;
using Unity.Mathematics;

namespace Breakout.System.Input
{
	[AlwaysSynchronizeSystem]
	[UpdateBefore(typeof(MoveSystem))]
	public class BallInputSystem : JobComponentSystem
	{
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);
			Entities
				.WithAll<BallTag, Parent>()
				.ForEach((Entity ballEntity, in BallInput ballInputData, in LocalToWorld localToWorld) =>
				{
					if (UnityEngine.Input.GetKeyDown(ballInputData.m_launchKey))
					{
						float3 position = localToWorld.Position;
						ecb.RemoveComponent<Parent>(ballEntity);
						ecb.RemoveComponent<LocalToParent>(ballEntity);
						ecb.RemoveComponent<BlockMovement>(ballEntity);
						ecb.SetComponent(ballEntity, new Translation() { Value = position });
					}
				})
				.Run();
				
			ecb.Playback(EntityManager);
			ecb.Dispose();

			return inputDeps;
		}
	}

}
