using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class PixelPerfectMoveSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		float dt = Time.DeltaTime;

		JobHandle jobHandle = Entities
			.ForEach((ref Translation translation, in PixelPerfectTranslationData ppTranslation, in MoveData moveData) =>
			{
				translation.Value = math.round(translation.Value + (moveData.m_direction * moveData.m_speed * dt));
			})
			.Schedule(inputDeps);

		return jobHandle;
	}
}
