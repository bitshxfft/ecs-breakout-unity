using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[AlwaysSynchronizeSystem]
public class PaddleSpawnSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		return default;
	}
}
