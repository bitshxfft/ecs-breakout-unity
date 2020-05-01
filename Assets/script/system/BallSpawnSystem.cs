using Unity.Entities;
using Unity.Jobs;

public class BallSpawnSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		float dt = Time.DeltaTime;
		EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
		
		JobHandle jobHandle = Entities
			.ForEach((Entity entity, ref BallSpawnData spawnData) =>
			{
				spawnData.m_delay -= dt;
				if (spawnData.m_delay <= 0.0f)
				{
					// #SteveD >>> spawn ball
					;
		
					// destroy request
					ecb.DestroyEntity(entity);
				}
			})
			.Schedule(inputDeps);
		
		ecb.Playback(EntityManager);
		ecb.Dispose();
		
		return jobHandle;
	}
}
