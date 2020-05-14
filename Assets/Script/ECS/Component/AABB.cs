using Unity.Entities;
using Unity.Mathematics;

namespace Breakout.Component
{
	[GenerateAuthoringComponent]
	public struct AABB : IComponentData
	{
		public CollisionLayer m_collisionLayer;
		public int2 m_bottomLeft;
		public int2 m_topRight;
	}
}
