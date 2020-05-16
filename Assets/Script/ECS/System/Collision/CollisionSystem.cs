using Breakout.Component.Collision;
using Unity.Entities;
using Unity.Transforms;

namespace Breakout.System.Collision
{
	public abstract class CollisionSystem<TCollider> : JobComponentSystem 
		where TCollider : struct, IComponentData
	{
		protected EntityQuery m_colliderQuery = default;

		// ----------------------------------------------------------------------------

		protected override void OnCreate()
		{
			base.OnCreate();

			m_colliderQuery = GetEntityQuery(
				ComponentType.ReadOnly<TCollider>(), 
				ComponentType.ReadOnly<Translation>(),
				typeof(CollisionEvent));
		}
	}
}
