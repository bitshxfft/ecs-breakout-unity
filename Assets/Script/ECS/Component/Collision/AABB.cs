using Unity.Entities;
using Unity.Mathematics;

namespace Breakout.Component.Collision
{
	[GenerateAuthoringComponent]
	public struct AABB : IComponentData
	{
		public CollisionLayer m_collisionLayer;
		public CollisionLayer m_collisionFilter;

		public float2 m_bottomLeft;
		public float2 m_topRight;
	}
}
