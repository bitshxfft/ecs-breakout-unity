using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;

[AlwaysSynchronizeSystem]
public class BallOutOfBoundsSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

		Entities
			.WithAll<BallTag>()
			.ForEach((Entity entity, in Translation translation) =>
		{
			// #SteveD >>> check for below screen bounds
			//			>> ecb.DestroyEntity(entity)
			// #SteveD >>> trigger next ball spawn 
		}).Run();

		ecb.Playback(EntityManager);
		ecb.Dispose();

		return default;
	}
}
