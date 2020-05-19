using Breakout.Component.Powerup;
using Breakout.Component.Prefab;
using Breakout.System.Collision;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Breakout.System.Spawn
{
	[UpdateAfter(typeof(PowerupCollisionResolutionSystem))]
	public class PowerupSpawnSystem : JobComponentSystem
	{
		private EndSimulationEntityCommandBufferSystem m_ecbSystem = default;
		private EntityQuery m_powerupPrefabQuery = default;
		
		// --------------------------------------------------------------------------------

		protected override void OnCreate()
		{
			base.OnCreate();

			m_ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
			m_powerupPrefabQuery = GetEntityQuery(ComponentType.ReadOnly<PowerupPrefab>());
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			EntityCommandBuffer.Concurrent ecb = m_ecbSystem.CreateCommandBuffer().ToConcurrent();
			NativeArray<PowerupPrefab> powerupPrefabs = m_powerupPrefabQuery.ToComponentDataArray<PowerupPrefab>(Allocator.TempJob);

			JobHandle jobHandle = Entities
				.WithDeallocateOnJobCompletion(powerupPrefabs)
				.ForEach((Entity entity, int entityInQueryIndex, in PowerupSpawnRequest spawnData) =>
				{
					Entity powerupEntity = ecb.Instantiate(entityInQueryIndex, powerupPrefabs[0].m_prefab);
					ecb.AddComponent(entityInQueryIndex, powerupEntity, spawnData.m_powerup);
					ecb.SetComponent(entityInQueryIndex, powerupEntity, 
						new Translation() 
						{ 
							Value = new float3(spawnData.m_position.x, spawnData.m_position.y, 0.0f) 
						});
					ecb.DestroyEntity(entityInQueryIndex, entity);
				})
				.Schedule(inputDeps);

			m_ecbSystem.AddJobHandleForProducer(jobHandle);
			return jobHandle;
		}
	}
}
