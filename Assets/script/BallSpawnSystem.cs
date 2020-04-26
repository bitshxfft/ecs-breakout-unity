using Unity.Entities;
using Unity.Jobs;

[AlwaysSynchronizeSystem]
public class BallSpawnSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		// #SteveD >>> spawn ball if none exist

		// #SteveD >>> spawn ball on request

		return default;
	}
}
