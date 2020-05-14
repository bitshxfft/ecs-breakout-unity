using Unity.Entities;
using Unity.Mathematics;

namespace Breakout.Component
{
	public struct BallSpawnRequest : IComponentData
	{
		public float2 m_position;
		public float2 m_direction;
		public float m_speed;
		public float m_delay;
		public bool m_attachToPaddle;
	}
}
