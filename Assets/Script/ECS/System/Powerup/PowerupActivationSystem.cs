using Unity.Entities;

namespace Breakout.System.Powerup
{
	public abstract class PowerupActivationSystem<TRequest> : JobComponentSystem 
		where TRequest : struct, IComponentData
	{
		protected EndSimulationEntityCommandBufferSystem m_ecbSystem = default;
		protected EntityQuery m_activationQuery = default;
		
		// --------------------------------------------------------------------------------

		protected override void OnCreate()
		{
			base.OnCreate();

			m_ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
			m_activationQuery = GetEntityQuery(ComponentType.ReadOnly<TRequest>());
		}
	}
}
