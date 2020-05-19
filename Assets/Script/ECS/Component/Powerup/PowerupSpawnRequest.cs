using Unity.Entities;
using Unity.Mathematics;

namespace Breakout.Component.Powerup
{
	public struct PowerupSpawnRequest : IComponentData
	{
		public Powerup m_powerup;
		public float2 m_position;
	}
}
