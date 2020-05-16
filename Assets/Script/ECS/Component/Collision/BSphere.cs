using Unity.Entities;

namespace Breakout.Component.Collision
{
	[GenerateAuthoringComponent]
	public struct BSphere : IComponentData
	{
		public CollisionLayer m_collisionLayer;
		public CollisionLayer m_collisionFilter;

		public float m_radius;
	}
}
