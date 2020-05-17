using Breakout.Component.Input;
using Breakout.Component.Movement;
using Breakout.Component.Prefab;
using Breakout.Component.Spawn;
using Breakout.Component.Tag;
using Breakout.Config;
using Breakout.System.Movement;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Breakout.System.Input
{
	[AlwaysSynchronizeSystem]
	[UpdateBefore(typeof(MoveSystem))]
	public class PaddleInputSystem : JobComponentSystem
	{
		private EntityQuery m_ballQuery = default;
		private EntityQuery m_ballRequestQuery = default;
		private EntityQuery m_brickQuery = default;
		private EntityQuery m_brickPrefabQuery = default;

		// ----------------------------------------------------------------------------

		protected override void OnCreate()
		{
			base.OnCreate();

			m_ballQuery = GetEntityQuery(ComponentType.ReadOnly<BallTag>());
			m_ballRequestQuery = GetEntityQuery(ComponentType.ReadOnly<BallSpawnRequest>());
			m_brickQuery = GetEntityQuery(ComponentType.ReadOnly<BrickTag>());
			m_brickPrefabQuery = GetEntityQuery(ComponentType.ReadOnly<BrickPrefab>());
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			EntityArchetype ballRequestArchetype = EntityManager.CreateArchetype(typeof(BallSpawnRequest));

			Entities
				.ForEach((ref Direction direction, in PaddleInput paddleInputData) =>
				{
					float x = 0;
					x += UnityEngine.Input.GetKey(paddleInputData.m_leftKey) ? -1.0f : 0.0f;
					x += UnityEngine.Input.GetKey(paddleInputData.m_rightKey) ? 1.0f : 0.0f;
					direction.m_direction = new float2(x, 0.0f);
				})
				.Run();

			if (m_ballQuery.CalculateEntityCount() == 0
				&& m_ballRequestQuery.CalculateEntityCount() == 0)
			{
				Entity spawnRequest = EntityManager.CreateEntity(ballRequestArchetype);
				EntityManager.SetComponentData<BallSpawnRequest>(
					spawnRequest,
					new BallSpawnRequest
					{
						m_attachToPaddle = true,
						m_delay = 0.0f,
						m_speed = GameConfig.k_defaultBallSpeed,
						m_direction = new float2(0.0f, 1.0f),
					});
			}

			if (m_brickQuery.CalculateEntityCount() == 0)
			{
				SpawnLevel();
			}

			return inputDeps;
		}

		private void SpawnLevel()
		{
			var prefabs = m_brickPrefabQuery.ToComponentDataArray<BrickPrefab>(Allocator.TempJob);
			if (prefabs.Length == 0)
			{
				prefabs.Dispose();
				return;
			}

			for (int y = 1; y <= 6; ++y)
			{
				Entity prefab = prefabs[0].m_prefab_level6;
				switch (y)
				{
					case 1: prefab = prefabs[0].m_prefab_level1; break;
					case 2: prefab = prefabs[0].m_prefab_level2; break;
					case 3: prefab = prefabs[0].m_prefab_level3; break;
					case 4: prefab = prefabs[0].m_prefab_level4; break;
					case 5: prefab = prefabs[0].m_prefab_level5; break;
				}

				for (int x = -8; x <= 8; ++x)
				{
					Entity brick = EntityManager.Instantiate(prefab);
					EntityManager.SetComponentData<Translation>(brick, new Translation { Value = new float3(x * 98, 140 + y * 48, 0.0f) });
				}
			}

			prefabs.Dispose();
		}
	}
}
