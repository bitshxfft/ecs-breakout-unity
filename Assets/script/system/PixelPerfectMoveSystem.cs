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
			.ForEach((ref Translation translation, ref PixelPerfectTranslation ppTranslation, in MoveData moveData) =>
			{
				// keep track of real position
				ppTranslation.m_realTranslation += (moveData.m_direction * moveData.m_speed * dt);
				// snap to pixel perfect position
				ppTranslation.m_pixelPerfectTranslation = math.round(ppTranslation.m_realTranslation);
				// update Tralslation component to pixel perfect position
				translation.Value = ppTranslation.m_pixelPerfectTranslation;
			})
		.Run();

		return default;
	}
}
