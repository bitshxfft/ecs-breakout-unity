using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[AlwaysSynchronizeSystem]
public class PixelPerfectMoveSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		float dt = Time.DeltaTime;

		Entities
			.ForEach((ref Translation translation, ref PixelPerfectTranslationData ppTranslation, in MoveData moveData) =>
			{
				translation.Value = math.round(translation.Value + (moveData.m_direction * moveData.m_speed * dt));
			})
		.Run();

		return default;
	}
}
